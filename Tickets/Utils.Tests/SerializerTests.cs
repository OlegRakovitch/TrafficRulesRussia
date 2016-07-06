﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using AppLogic.Interfaces;
using AppLogic.Enums;
using AppLogic;
using AppData;
using Windows.ApplicationModel;
using System.IO;
using System.Diagnostics;
using Utils;

namespace Utils.Tests
{
    [TestClass]
    public class SerializerTests
    {
        class SP : ISessionParameters {

            public bool Shuffle {
                get;
                set;
            }

            public int[] TicketNums {
                get;
                set;
            }

            public QuestionsGenerationMode Mode {
                get;
                set;
            }
        }

        [ClassInitialize]
        public static void Initialize( TestContext context ) {
            Resources.ConnectionString = Path.Combine(Package.Current.InstalledLocation.Path, Resources.DBFileName);
        }

        private void Serialize<T>(object obj) {
            // arrange
            string serializedObj = null;
            T deserializedObj = default(T);

            // act
            if(obj != null) {
                if((serializedObj = Serializer.SerializeToString(obj)) != null) {
                    deserializedObj = Serializer.DeserializeFromString<T>(serializedObj);
                }
            }

            // assert
            Assert.IsNotNull(serializedObj, "serializedObj == null");
            Assert.IsNotNull(deserializedObj, "deserializedObj == null");
        }

        [TestMethod]
        public void CanSerializeAnyObject() {
            Serialize<int>(1);
            Serialize<double>(1.1);
            Serialize<string>("TestString");
        }

        [TestMethod]
        public void CanSerializeUserObject() {
            ISession session;
            SessionFactory.CreateSession(new SP() { Mode = QuestionsGenerationMode.RandomTicket}, out session);
            Serialize<Session>(session);
        }
    }
}
