﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BatchDataEntry.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBatchDataEntry
{
    [TestClass]
    public class CacheTest
    {
        public static readonly string _PATH = Path.Combine(Directory.GetCurrentDirectory(), "testcache.ini");
        public static readonly string _SECTION = "testSection";

        [TestMethod]
        public void TestInit()
        {
            try
            {
                Cache.CreateFile(_PATH);
                Assert.IsTrue(File.Exists(_PATH));
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestAddSection()
        {
            try
            {
                Cache.AddSection(_PATH, _SECTION);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestAddSingleKey()
        {
            try
            {
                Cache.AddKeyToSection(_PATH, _SECTION, "test", "123456789");
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestAddMultipleSection()
        {
            try
            {
                List<string> sections = new List<string>();
                sections.Add("section2");
                sections.Add("section3");
                sections.Add("section4");

                Cache.AddMultipleSection(_PATH, sections);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [TestMethod]
        public void TestAddMultipleKey()
        {
            try
            {
                string[] keys = {"key_1", "key_2", "key_3"};
                string[] values = {"value_1", "value_2", "value_3"};

                Cache.AddMultipleKeyToSection(_PATH, _SECTION, keys, values);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
                throw;
            }
        }

        [TestMethod]
        public void TestGetKey()
        {
            try
            {
                Dictionary<string, string> res = Cache.GetKey(_PATH, _SECTION, "test");
                Assert.AreEqual("123456789", res.ElementAt(0).Value);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
                
            }      
        }

        [TestMethod]
        public void TestAddSerializedArray()
        {
            string[] tmp = new string[] { "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8" };
            try
            {              
                Cache.AddSeriazliedValueToSection(_PATH, _SECTION, "arrayprova", tmp);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);

            }
        }

        [TestMethod]
        public void TestGetSerializedArray()
        {
            string[] res;
            string[] exp = new string[] { "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8" };
            try
            {
                res = Cache.GetSerializedValue(_PATH, _SECTION, "arrayprova");

                if(res.Length == 0)
                    Assert.IsTrue(false, "Array recuperato dal file vuoto...");

                for (int i = 0; i < res.Length; i++)
                {
                    Assert.AreEqual(res[i], exp[i]);
                }

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);

            }
        }
    }
}
