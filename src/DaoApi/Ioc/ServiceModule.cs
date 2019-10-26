using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Zuto.Uk.Sample.API.Controllers;
using Zuto.Uk.Sample.API.Models;
using Zuto.Uk.Sample.API.Repositories;

namespace Zuto.Uk.Sample.API.Ioc
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobsRepo>().As<IJobsRepo>();
            builder.RegisterType<BuddiesRepo>().As<IBuddiesRepo>();
            builder.RegisterType<BuddyFinder>().As<IBuddyFinder>();
            builder.RegisterType<JobScheduler>().As<IJobScheduler>();
        }
    }
}
