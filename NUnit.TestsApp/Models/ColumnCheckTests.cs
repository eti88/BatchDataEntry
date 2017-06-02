using NUnit.Framework;
using System;
using BatchDataEntry.Business;

namespace NUnit.TestsApp.Models
{
    [TestFixture()]
    public class ColumnCheckTests
    {
        [Test()]
        public void CheckisNotVoidTestSuccess()
        {
            string[] val = new string[] { "abc", "ciao", "test123", "aaaaaaaaaaa"};
            foreach(string s in val)
            {
                Assert.IsTrue(Utility.IsNotVoid(s));
            }
        }

        [Test()]
        public void CheckisNotVoidTestFail()
        {
            string[] val = new string[] { " ", "", "  ", "  " };
            foreach (string s in val)
            {
                Assert.IsFalse(Utility.IsNotVoid(s));
            }
        }

        [Test()]
        public void CheckTelTestSuccess()
        {
            string[] val = new string[] { "1234567890", "3337777777", "7896540000" };
            foreach (string s in val)
            {
                Assert.IsTrue(Utility.IsValidTelephone(s));
            }
        }

        [Test()]
        public void CheckTelTestFail()
        {
            string[] val = new string[] { "123-4567890", "+3337777777", "789!540000", "333777777711", "33377777" };
            foreach (string s in val)
            {
                Assert.IsFalse(Utility.IsValidTelephone(s));
            }
        }

        [Test()]
        public void CheckEmailTestSuccess()
        {
            string[] val = new string[]
            {
                "email@example.com",
                "firstname.lastname@example.com",
                "email@subdomain.example.com",
                "firstname+lastname@example.com",
                "email@123.123.123.123",
                "email@[123.123.123.123]",
                "1234567890@example.com",
                "email@example.name",
                 "email@example.museum",
                 "email@example.co.jp",
                "firstname-lastname@example.com"
            };
            foreach (string s in val)
            {
                bool r = Utility.IsValidEmail(s);

                if (!r)
                    Console.WriteLine("Fail on " + s);

                Assert.IsTrue(r);
            }
        }

        [Test()]
        public void CheckEmailTestFail()
        {
            string[] val = new string[]
            {
                "Joe Smith <email@example.com>",
                "email.example.com",
                "email@example@example.com",
                ".email@example.com",
                "email.@example.com",
                "email..email@example.com",
                "email@example.com (Joe Smith)",
                "email@example",
                "email@-example.com",
                "email@example..com",
                "Abc..123@example.com"
            };

            foreach (string s in val)
            {
                bool r = Utility.IsValidEmail(s);
                Assert.IsFalse(r);
            }
        }

        [Test()]
        public void CheckDateTestSuccess()
        {
            string[] val = new string[]
            {
                "1990-10-11",
                "2009-05-15",
                "2015-03-02"
            };

            foreach (string s in val)
            {
                bool r = Utility.IsValidDate(s, "yyyy-MM-dd");
                if (!r)
                    Console.WriteLine("Fail on " + s);
                Assert.IsTrue(r);
            }
        }

        [Test()]
        public void CheckDateTestFail()
        {
            string[] val = new string[]
            {
                "15-10-11",
                "1-3-2015",
                "aa11/30",
                "03/13/2013",
                "2015/04/01",
                "10-Nov-2015",
                "10102014"
            };

            foreach (string s in val)
            {
                Assert.IsFalse(Utility.IsValidDate(s, "yyyy-MM-dd"));
            }
        }
    }
}
