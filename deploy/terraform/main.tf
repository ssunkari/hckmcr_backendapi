terraform {
  backend "s3" {
    bucket = "zuto-terraform-state-files"
    key    = "services/sample-web-api/sample-web-api.tfstate"
    region = "eu-west-2"
    acl    = "bucket-owner-full-control"
  }
}

provider "aws" {
  region = "eu-west-2"
}

data "aws_vpc" "vpc" {
  filter {
    name   = "tag:Name"
    values = ["vpc.eu-west-2"]
  }
}

data "template_file" "sample-web-api_task_definition" {
  template = "${file("task-definition.json.tpl")}"

  vars {
    service_name = "sample-web-api"
    image_name   = "525344447431.dkr.ecr.eu-west-2.amazonaws.com/sample-web-api:${var.service_version}"
    cpu          = 128
    memory       = 256
    environment  = "${terraform.workspace}"
  }
}

module "sample-web-api_ecs_service" {
  source                                = "git@github.com:carloan4u/terraform-aws-ecs-service.git?ref=0.0.11"
  environment                           = "${terraform.workspace}"
  vpc_id                                = "${data.aws_vpc.vpc.id}"
  service_name                          = "sample-web-api"
  ecs_cluster_name                      = "ecs-cluster-internal-${terraform.workspace}"
  task_definition_container_definitions = "${data.template_file.rate_modeller_task_definition.rendered}"
  load_balancer_name                    = "ecs-cluster-internal-${terraform.workspace}-alb"
  create_dns_record                     = true
  alarms_sns_topic                      = "product-integration-sns-topic"
}
