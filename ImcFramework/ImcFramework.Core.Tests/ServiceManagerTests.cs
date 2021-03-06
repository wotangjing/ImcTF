﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImcFramework.WcfInterface;
using Quartz;
using Quartz.Impl;
using ImcFramework.Ioc;
using ImcFramework.Core.Quartz;
using System.Reflection;
using System.Linq;
using ImcFramework.Core.WcfService;
using Moq;
using System.ServiceModel;
using ImcFramework.LogPool;
using ImcFramework.Core.Distribution;
using ImcFramework.WcfInterface.TransferMessage;
using ImcFramework.Core.MqModuleExtension;

namespace ImcFramework.Core.Tests
{
    [TestClass]
    public class ServiceManagerTests
    {
        private Assembly asm = null;
        private IIocManager ioc = null;

        //在运行每个测试之前，使用 TestInitialize 来运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
            asm = typeof(ServiceManager).Assembly;
            ioc = Ioc.IocManager.Instance;
        }

        //在每个测试运行完之后，使用 TestCleanup 来运行代码
        [TestCleanup()]
        public void MyTestCleanup()
        {
            asm = null;
            ioc = null;
        }

        [TestMethod]
        public void register_asm_can_resolve_the_IServiceModule()
        {
            ioc.RegisterAssemblyAsInterfaces(typeof(ILoggerPoolFactory).Assembly);
            ioc.RegisterAssemblyAsInterfaces(asm);

            var serviceModules = ioc.Resolve<IEnumerable<IServiceModule>>();
            foreach (var md in serviceModules)
            {
                Console.WriteLine(md.Name);
            }

            Assert.IsNotNull(serviceModules);
            Console.WriteLine(serviceModules.Count());
        }

        [TestMethod]
        public void register_asm_can_resolve_the_IModuleExtension()
        {
            ioc.RegisterAssemblyAsInterfaces(asm);

            var serviceModules = ioc.Resolve<IEnumerable<IModuleExtension>>();
            foreach (var md in serviceModules)
            {
                Console.WriteLine(md.GetType().ToString());
            }

            Assert.IsNotNull(serviceModules);
            Console.WriteLine(serviceModules.Count());
        }

        [TestMethod]
        public void register_asm_can_resolve_ICommandInvoker()
        {
            ioc.RegisterAssemblyAsInterfaces(asm);

            Mock<IScheduler> moq = new Mock<IScheduler>();
            ioc.Register<IScheduler>(moq.Object);// 依赖于IScheduler，由于没有启动ISchedule，则不会有，故mock一个。

            var cmdInvoker = ioc.Resolve<ICommandInvoker>();

            Assert.IsNotNull(cmdInvoker);
        }

        [TestMethod]
        public void register_asm_can_resolve_ILoggerPoolFactory()
        {
            ioc.RegisterAssemblyAsInterfaces(asm);

            var logPool = ioc.Resolve<ILoggerPoolFactory>();

            Assert.IsNotNull(logPool);
        }

        [TestMethod]
        public void register_asm_can_resolve_IClientConnector()
        {
            ioc.RegisterAssemblyAsInterfaces(asm);

            var wcf = ioc.Resolve<IClientConnector>();

            Assert.IsNotNull(wcf);
        }

        [TestMethod]
        public void register_asm_can_resolve_IDistributionFacility_T()
        {
            ioc.RegisterAssemblyAsInterfaces(asm);
            ioc.RegisterGeneric(typeof(MsmqDistribution<>), typeof(IDistributionFacility<>));

            var mq = ioc.Resolve<IDistributionFacility<MessageEntity>>();

            Assert.IsNotNull(mq);
        }

        [TestMethod]
        public void register_asm_can_resolve_ITransferMessage()
        {
            ioc.RegisterAssemblyAsInterfaces(typeof(ITransferMessage).Assembly);
            ioc.RegisterAssemblyAsInterfaces(asm);

            var list = ioc.Resolve<IEnumerable<ITransferMessage>>();
            foreach (var item in list)
            {
                Console.WriteLine(item.GetType().ToString());
            }

            Assert.AreEqual(2, list.Count());
        }

        //[TestMethod]
        //public void register_asm_can_resolve_IMessageEntityCallback()
        //{
        //    ioc.RegisterAssemblyAsInterfaces(typeof(ITransferMessage).Assembly);
        //    ioc.RegisterAssemblyAsInterfaces(asm);

        //    var list = ioc.Resolve<IEnumerable<ITransferMessageCallback<MessageEntity>>>();
        //    foreach (var item in list)
        //    {
        //        Console.WriteLine(item.GetType().ToString());
        //    }

        //    Assert.AreEqual(1, list.Count());
        //}

        [TestMethod]
        public void register_asm_can_resolve_IIDistributionFacility_T()
        {
            List<IDistributionFacility<ITransferMessage>> m_MqDistributions = new List<IDistributionFacility<ITransferMessage>>();
            ioc.RegisterAssemblyAsInterfaces(typeof(ITransferMessage).Assembly);
            ioc.RegisterAssemblyAsInterfaces(asm);

            ioc.RegisterGeneric(typeof(MsmqDistribution<>), typeof(IDistributionFacility<>));

            var list = ioc.Resolve<IEnumerable<ITransferMessage>>();
            foreach (var item in list)
            {
                Type generic = typeof(IDistributionFacility<>);
                generic = generic.MakeGenericType(new Type[] { item.GetType() });

                var obj = ioc.Resolve(generic);
                m_MqDistributions.Add((IDistributionFacility<ITransferMessage>)obj);

                Console.WriteLine(obj.GetType().ToString());
            }

            Assert.AreEqual(2, m_MqDistributions.Count());
        }

        [TestMethod]
        public void register_asm_can_resolve_IProgressInfoManager()
        {
            ioc.Register<MutilUserProgress.IProgressInfoManager>(MutilUserProgress.ProgressInfoManager.Instance);

            var mgr = ioc.Resolve<MutilUserProgress.IProgressInfoManager>();

            Assert.IsNotNull(mgr);
        }

        [TestMethod]
        public void register_asm_can_resolve_IProgressInfoManager_is_singleton()
        {
            ioc.Register<MutilUserProgress.IProgressInfoManager>(MutilUserProgress.ProgressInfoManager.Instance);

            var mgr = ioc.Resolve<MutilUserProgress.IProgressInfoManager>();
            var mgr2 = ioc.Resolve<MutilUserProgress.IProgressInfoManager>();

            Assert.AreSame(mgr, mgr2);
        }
    }
}
