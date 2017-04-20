using NUnit.Framework;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace BatchDataEntry.Models.Tests
{
    [TestFixture()]
    public class DbCredentialTests
    {
        [Test()]
        public void DbCredentialTest()
        {
            DbCredential d = new DbCredential();
            Assert.IsNotNull(d);
        }

        [Test()]
        public void DbCredentialTest1()
        {
            DbCredential d = new DbCredential(true, "utest", "utest", "localhost", "test");
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Use == true);
            Assert.IsTrue(d.User.Equals("utest"));
            Assert.IsTrue(d.Password.Equals("utest"));
            Assert.IsTrue(d.Address.Equals("localhost"));
            Assert.IsTrue(d.Dbname.Equals("test"));
        }

        [Test()]
        public void TestConnectionTestFail()
        {
            DbCredential d = new DbCredential(true, "utest", "utest", "localhost", "test");
            Assert.IsFalse(d.TestConnection());
        }

        [Test()]
        public void TestConnectionTestSuccess()
        {
            string user = @"unitTest";
            string server = @"localhost\SQLEXPRESS";
            string dbname = @"db_BatchDataEntry_unitTest";
            DbCredential d = new DbCredential(true, user, user, server, dbname);
            Assert.IsTrue(d.TestConnection());
        }

        [Test()]
        public void ToSecureStringTest()
        {
            SecureString s = DbCredential.ToSecureString("abcde");
            Assert.IsTrue(DbCredential.ToInsecureString(s).Equals("abcde"));
        }

        [Test()]
        public void ToInsecureStringTest()
        {
            char[] c = new char[] { 'a', 'b', 'c', 'd', 'e' };
            SecureString sr = new SecureString();
            foreach (char x in c)
                sr.AppendChar(x);
            Assert.IsTrue(DbCredential.ToInsecureString(sr).Equals("abcde"));
        }
    }
}