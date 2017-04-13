using NUnit.Framework;
using BatchDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BatchDataEntry.Models.Tests
{
    [TestFixture()]
    public class ModelloTests
    {
        [Test()]
        public void ModelloTest()
        {
            Modello m = new Modello();
            Assert.IsNotNull(m);
        }

        [Test()]
        public void ModelloTest1()
        {
            Modello m = new Modello("unitModel", false, new ObservableCollection<Campo>());
            Assert.IsNotNull(m);
        }

        [Test()]
        public void ModelloTest2()
        {
            Modello m = new Modello(1, "unitModel", false, new ObservableCollection<Campo>());
            Assert.IsNotNull(m);
        }

        [Test()]
        public void ModelloTest3()
        {
            Modello m = new Modello(1, "unitModel", false, new ObservableCollection<Campo>(), "C:\\boh\file.csv", ";");
            Assert.IsNotNull(m);
        }

        [Test()]
        public void ModelloTest4()
        {
            Modello m = new Modello(1, "unitModel", false, new ObservableCollection<Campo>(), "C:\\boh\file.csv", ";");
            Modello mcopy = new Modello(m);
            Assert.IsNotNull(mcopy);
        }

        [Test()]
        public void RevertTest()
        {
            Modello m = new Modello(1, "unitModel", false, new ObservableCollection<Campo>(), "C:\\boh\file.csv", ";");
            m.Id = 100;
            m.Nome = "isaodnoand";
            m.Revert();
            Assert.IsTrue(m.Id == 1 && m.Nome.Equals("unitModel"));
        }

        [Test()]
        public void EqualsTest()
        {
            Modello modello = new Modello(0, "ModelloTest", false, new ObservableCollection<Campo>(), string.Empty, string.Empty);
            Modello copy = new Modello(modello);
            bool actual = modello.Equals(copy);
            Assert.AreEqual(true, actual);
        }

        [Test()]
        public void EqualsTest2()
        {
            Modello modello = new Modello(0, "ModelloTest1", true, new ObservableCollection<Campo>(), "C:/test/1/ac.csv", ";");
            Modello modello2 = new Modello(9, "ModelloTest2", true, new ObservableCollection<Campo>(), "D:/test/3/ac.csv", ";");
            bool actual = modello.Equals(modello2);
            Assert.AreEqual(false, actual);
        }

        [Test()]
        public void GetHashCodeTest()
        {
            Modello m = new Modello(1, "unitModel", false, new ObservableCollection<Campo>(), "C:\\boh\file.csv", ";");
            Modello m2 = new Modello(1, "unitModel", false, new ObservableCollection<Campo>(), "C:\\boh\file.csv", ";");
            int h1 = m.GetHashCode();
            int h2 = m2.GetHashCode();
            Assert.Equals(h1, h2);
        }
    }
}