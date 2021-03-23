using Mit_Oersted.Domain.Commands.Invoices;
using Mit_Oersted.Domain.Entities;
using Mit_Oersted.Domain.Entities.Models;
using Mit_Oersted.Domain.Repository;
using Mit_Oersted.Domain.Repository.Implementations;
using Mit_Oersted.DummyDataConsole.Models;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Mit_Oersted.DummyDataConsole.Tasks
{
    public class DoWork
    {
        private static readonly Random Random = new();

        public async Task<DummyDataUserModel[]> ReadDummyDataTask(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    Log.Error($"Config file not found: '{ jsonFilePath }' in '{ MethodBase.GetCurrentMethod().Name }'");
                    return null;
                }

                var jsonFileLines = await File.ReadAllTextAsync(jsonFilePath);
                var jsonObject = JObject.Parse(jsonFileLines);
                var dummyData = jsonObject.GetArray<DummyDataUserModel>("Users");

                return dummyData;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in '{ MethodBase.GetCurrentMethod().Name }'");
                return null;
            }
        }

        public List<object> SortDummmyDataTask(DummyDataUserModel[] dummyData)
        {
            try
            {
                if (dummyData == null)
                {
                    Log.Error($"Model is null in '{ MethodBase.GetCurrentMethod().Name }'");
                    return null;
                }

                var users = new List<UserModel>();
                var addresses = new List<AddressModel>();
                var invoices = new List<CreateInvoiceCommand>();

                foreach (DummyDataUserModel dummyUser in dummyData)
                {
                    var id = RandomString();

                    users.Add(new UserModel
                    {
                        Id = id,
                        Name = dummyUser.Name,
                        Address = dummyUser.PrimaryAddress.Address,
                        Email = dummyUser.Email,
                        Phone = dummyUser.Phone
                    });

                    foreach (DummyDataAddressModel dummyAddress in dummyUser.Addresses)
                    {
                        addresses.Add(new AddressModel
                        {
                            Id = dummyAddress.Id,
                            AddressString = dummyAddress.Address,
                            UserId = id
                        });

                        foreach (DummyDataInvoiceModel dummyInvoice in dummyAddress.Invoices)
                        {
                            DirectoryInfo directoryInfo = new(@$"C:\temp\skole\documents\{dummyAddress.Id}");

                            var fileData = new byte[1];
                            var cost = string.Empty;
                            foreach (FileInfo file in directoryInfo.GetFiles($"forbrug-{dummyInvoice.Date.Year}-{dummyInvoice.Date.Month}-{dummyInvoice.Date.Day}-*.pdf"))
                            {
                                fileData = File.ReadAllBytes(file.FullName);
                                cost = file.Name.Replace($"forbrug-{dummyInvoice.Date.Year}-{dummyInvoice.Date.Month}-{dummyInvoice.Date.Day}-", "").Replace(".pdf", "");
                            }

                            invoices.Add(new CreateInvoiceCommand
                            {
                                FolderName = dummyAddress.Id,
                                FileName = $"forbrug-{dummyInvoice.Date.Year}-{dummyInvoice.Date.Month}-{dummyInvoice.Date.Day}.pdf",
                                MetaData = new()
                                {
                                    { "monthlyCost", cost }
                                },
                                File = fileData
                            });
                        }
                    }
                }

                var result = new List<object>
                {
                    users,
                    addresses,
                    invoices
                };

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in '{ MethodBase.GetCurrentMethod().Name }'");
                return null;
            }
        }

        public async Task HandelDummmyDataTask(List<object> sortedData, string configFile)
        {
            var databaseEntities = new DatabaseEntities(configFile);
            var userRepository = new UserRepository(databaseEntities);
            var invoiceRepository = new InvoiceRepository(databaseEntities, configFile);
            var addressRepository = new AddressRepository(databaseEntities);

            //await userRepository.AddAsync();

            //TODO: Call add user
            //TODO: Call add address
            //TODO: Call add invoice
        }

        private static string RandomString()
        {
            var passwordLength = 20;

            int seed = Random.Next(1, int.MaxValue);
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var chars = new char[passwordLength];
            var rd = new Random(seed);

            for (var i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
