﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Azure;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace FileLogging
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        static TextWriter tw = null;

        protected void Application_Start()
        {
            tw = new StreamWriter("C:\\Temp\\Botlog.txt", true);

            // register various services we will need for the operation of our bot
            Conversation.UpdateContainer(
              builder =>
              {
                  // these first two registrations allow us to manage bot state
                  builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
                  var store = new InMemoryDataStore();


                  builder.Register(c => store)
                            .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                            .AsSelf()
                            .SingleInstance();

                  // register our logger here
                  builder.RegisterType<FileActivityLogger>().AsImplementedInterfaces()
                    .InstancePerDependency().WithParameter("inputFile", tw);

              });

                     
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}