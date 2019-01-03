[{
  "name": "${service_name}",
  "image": "${image_name}",
  "cpu": ${cpu},
  "memory": ${memory},
  "essential": true,
  "portMappings": [{
    "containerPort": 80
  }],
  "environment": [{
      "name": "ASPNETCORE_ENVIRONMENT",
      "value": "${environment}"
    }
  ]
}]
