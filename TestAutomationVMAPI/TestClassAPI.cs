using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NUnit.Framework;
using RestSharp;

namespace TestAutomationVMAPI
{
    [TestFixture]
    public class TestClassAPI
    {
        private static RestClient client = new RestClient("https://localhost:7022/VMPool");
        private RestRequest request = new();
        private RestResponse response = new();


        /*
         * Server Clean Up: Runs before Test Suite Execution
         */
        [OneTimeSetUp]
        public void cleanUp_ServerRefresh()
        {
            try
            {
                // arrange
                request = new RestRequest("/ServerRefresh/admin/123456", Method.Post);
                response = client.Execute(request);

                // assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.IsTrue(response.Content.Contains("Server Refreshed Successfully"));
            }
            catch (Exception e)
            {
                Console.WriteLine(response.Content + "\nException: " + e.StackTrace);
            }
        }

        /*
         * Positive Case:
         * User successfully Checkout in VM
         * Gets IP address
         */
        [Test, Order(1)]
        public void test_vmCheckout()
        {
            // arrange
            request = new RestRequest("/Checkout/user1", Method.Get);
                                        
            // act
            response = client.Execute(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.AreEqual(response.Content.Split(':')[1].Trim(), "user1");
            Assert.IsTrue(response.Content.Split(':')[3].Trim().Contains("10.10.10"), "user1 successfully checked-out in VM: "+ response.Content.Split(':')[3].Trim());
        }

        /*
         * Positive Case:
         * User successfully Checkin from VM
         * Returns: VM Usage time
         * free VM in VMPool
         */
        [Test, Order(2)]
        public void test_vmCheckin()
        {
            // arrange
            request = new RestRequest("/Checkin/user1", Method.Post);

            // act
            response = client.Execute(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.AreEqual(response.Content.Split(':')[1].Trim(), "user1");
            Assert.IsTrue(response.Content.Split(':')[3].Trim().Contains("10.10.10"), 
                "user1 successfully Checked-in from VM: "+ response.Content.Split(':')[3].Trim() + " : VM Usage mins: "+ response.Content.Split(':')[5].Trim());
        }

        /*
         * Negative Case:
         * User Checkout on 2nd VM, when already reserved 1 VM from Pool
         */
        [Test, Order(3)]
        public void test_vmMultipleCheckout()
        {
            try
            {
                // arrange
                request = new RestRequest("/Checkout/user2", Method.Get);

                // act
                response = client.Execute(request);

                // assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.AreEqual(response.Content.Split(':')[1].Trim(), "user2");
                Assert.IsTrue(response.Content.Split(':')[3].Trim().Contains("10.10.10"));

                request = new RestRequest("/Checkout/user2", Method.Get);
                response = client.Execute(request);

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.AreEqual(response.Content.Split(':')[2].Trim(), "Already Checked-out with VM");
                Assert.IsTrue(response.Content.Split(':')[3].Trim().Contains("10.10.10"));
            }
            catch (Exception e)
            {
                Console.WriteLine(response.Content +"\nException: "+e.StackTrace);
            }
        }

        /*
         * Negative Case:
         * User tries to Checkin when Checkout to No VM
         */
        [Test, Order(4)]
        public void test_vmCheckin_WhenNoCheckout()
        {
            try
            {
                // Refresh Server
                request = new RestRequest("/Checkin/user3", Method.Post);
                response = client.Execute(request);

                // assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.IsTrue(response.Content.Contains("Sorry: you're not checked-out in any of the VM"));
            }
            catch (Exception e)
            {
                Console.WriteLine(response.Content + "\nException: " + e.StackTrace);
            }
        }

        /*
         * Negative Case:
         * Inavlid Username
         */
        [Test, Order(5)]
        public void test_vmCheckout_with_InvalidUser()
        {
            // arrange
            request = new RestRequest("/Checkout/*&%$#@", Method.Get);

            // act
            response = client.Execute(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content.Contains("Sorry: No Authorization"));
        }

        /*
         * Negative Case:
         * User tries to Checkout, When All VMs are reserved
         */

        [Test, Order(6)]
        public void test_vmCheckout_When_No_VM_Available()
        {
            try
            {
                // arrange
                request = new RestRequest("/ServerRefresh/admin/123456", Method.Post);
                response = client.Execute(request);

                // assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
                Assert.IsTrue(response.Content.Contains("Server Refreshed Successfully"));

                bool flag = true;
                int i = 1;

                while(flag)
                {
                    request = new RestRequest("/Checkout/user"+i, Method.Get);
                    response = client.Execute(request);
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        i++;
                        flag = true;
                    }
                    else
                        flag = false;
                }

                // assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(response.Content.Contains("Sorry: No VM is available - Please retry after some time"));
            }
            catch (Exception e)
            {
                Console.WriteLine(response.Content + "\nException: " + e.StackTrace);
            }
        }

    }
}
