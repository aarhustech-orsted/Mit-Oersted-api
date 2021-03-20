using AutoFixture;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;


namespace Mit_Oersted.WebApi.Test.Controllers
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class UserControllerTest
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }


    }
}
