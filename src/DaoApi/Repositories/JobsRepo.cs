﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Zuto.Uk.Sample.API.Models;

namespace Zuto.Uk.Sample.API.Repositories
{
    public class JobsRepo :IJobsRepo
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private  const string TableName = "hackmcr_jobs";

        public JobsRepo(IAmazonDynamoDB dbClient)
        {
            _dynamoDbClient = dbClient;
        }

        public async Task<List<JobsModel>> GetAll()
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            var response = await _dynamoDbClient.ScanAsync(request);
            return response.Items.Select(Map).ToList();
        }

        public async Task<List<JobsModel>> GetJobsByPhoneNumber(string phoneNumber)
        {
            Amazon.DynamoDBv2.Model.Condition cond = new Condition();
            cond.ComparisonOperator = "CONTAINS";
            cond.AttributeValueList = new List<AttributeValue>() { new AttributeValue { S = phoneNumber  } };

            try
            {
                ScanResponse scRes = await _dynamoDbClient.ScanAsync(new ScanRequest { TableName = TableName, ScanFilter = new Dictionary<string, Condition>() { { "MobileNumber", cond } } });

                return scRes.Items.Any() ? scRes.Items.Select(Map).OrderByDescending(x=>x.TimestampRequested).ToList() : null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task DeleteAllJobs()
        {
            var request = new ScanRequest
            {
                TableName = TableName
            };

            var response = await _dynamoDbClient.ScanAsync(request);

            var items = response.Items.Select(Map);
            foreach (var item in items)
            {
                var deleteRequest = new DeleteItemRequest
                {
                    TableName = TableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        ["Id"] = new AttributeValue { S = item.Id }
                    }
                };

                await _dynamoDbClient.DeleteItemAsync(deleteRequest);
            }
        }

        public async Task<JobsModel> GetJobsByIdMatch(string idPrefix)
        {
            Amazon.DynamoDBv2.Model.Condition cond = new Condition();
            cond.ComparisonOperator = "CONTAINS";
            cond.AttributeValueList = new List<AttributeValue>() { new AttributeValue { S = idPrefix } };

            try
            {
                ScanResponse scRes = await _dynamoDbClient.ScanAsync(new ScanRequest { TableName = TableName, ScanFilter = new Dictionary<string, Condition>() { { "Id", cond } } });

                return scRes.Items.Select(Map).FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task Update(JobsModel jobsModel, Dictionary<string, AttributeValueUpdate> attributeValueUpdates)
        {
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["Id"] = new AttributeValue { S = jobsModel.Id }
                },
               AttributeUpdates = attributeValueUpdates
            };

            await _dynamoDbClient.UpdateItemAsync(updateItemRequest);
        }

        public async Task CreateJob(JobsModel model)
        {
            
            var putItemRequest = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    ["Id"] = new AttributeValue { S = model.Id },
                    ["MobileNumber"] = new AttributeValue { S = model.MobileNumber },
                    ["Name"] = new AttributeValue { S = model.Name },
                    ["Location"] = new AttributeValue { S = model.Location },
                    ["Lat"] = new AttributeValue { S = model.Lat },
                    ["Long"] = new AttributeValue { S = model.Long },
                    ["TimestampRequested"] = new AttributeValue { S = model.TimestampRequested.ToString("yyyy-MM-dd HH:mm:ss") },
                    ["TimestampRequiredFor"] = new AttributeValue { S = model.TimestampRequiredFor.ToString("yyyy-MM-dd HH:mm:ss") },
                    ["Message"] = new AttributeValue { S = model.Message },
                    ["TranslatedMessage"] = new AttributeValue { S = model.TranslatedMessage },
                    ["LanguageRequested"] = new AttributeValue { S = model.LanguageRequested },
                    ["Disabilities"] = new AttributeValue { SS = model.Disabilities }
                }
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);

        }

        private JobsModel Map(Dictionary<string, AttributeValue> result)
        {
            var dateTimeRequested = result["TimestampRequested"].S;
            var timestampRequestedFor = result["TimestampRequiredFor"].S;
            var bob = DateTime.Parse(timestampRequestedFor, CultureInfo.InvariantCulture);
            var job = new JobsModel
            {
                Id = result["Id"].S,
                Name = result["Name"].S,
                MobileNumber = result["MobileNumber"].S,
                Location = result["Location"].S,
                Lat = result["Lat"].S,
                Long = result["Long"].S,
                TimestampRequested = DateTime.Parse(dateTimeRequested, CultureInfo.InvariantCulture),
                TimestampRequiredFor = DateTime.Parse(timestampRequestedFor, CultureInfo.InvariantCulture),
                Message = result["Message"].S,
                TranslatedMessage = result["TranslatedMessage"].S,
                LanguageRequested = result["LanguageRequested"].S,
                Disabilities = result["Disabilities"].SS
            };
            if (result.ContainsKey("BuddiesAccepted"))
                job.BuddiesAccepted = result["BuddiesAccepted"].SS;
            if (result.ContainsKey("JobConfirmed"))
                job.JobConfirmed = result["JobConfirmed"].BOOL;
            if (result.ContainsKey("JobConfirmedTo"))
                job.JobConfirmedTo = result["JobConfirmedTo"].S;

            return job;
        }
    }
}