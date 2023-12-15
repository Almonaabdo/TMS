using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class InvoiceGeneratorModel
{

    private readonly TmsDbContext _dbContext = DbContextSingleton.Instance;
    private readonly LoggerModel _loggerModel = LoggerModel.Instance;

    public InvoiceGeneratorModel()
    {
        
    }

    /// <summary>
    /// Method to create an invoice
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="tripCost"></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    /// 
    /*
    * METHOD NAME: GenerateInvoice
    * DESCRIPTION: Creates an invoice
    * PARAM: orderID
    * RETURN: null
    */
    public Invoice GenerateInvoice(string orderId)
    {
        try
        {
            if (int.TryParse(orderId, out var id))
            {
                try
                {
                    if (_dbContext.Orders != null)
                    {
                        var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == id);

                        if (order != null)
                        {
                            var invoice = new Invoice
                            {
                                OrderId = order.OrderId,
                                //Amount
                                DateCompleted = (DateTime)order.DateCompleted!,
                                JobType = order.JobType,
                                VanType = order.VanType,
                                Quantity = order.Quantity,


                            };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                } 
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating invoice: {ex.InnerException}");
        }

        return null!;
    }


    /// <summary>
    /// Method to create a text file invoice with all details included
    /// </summary>
    /// <param name="invoice">The invoice to create file for</param>
    /// 
    /*
    * METHOD NAME: GenerateTxt
    * DESCRIPTION: Creates a text file invoice with all detials included
    * PARAM: invoice
    * RETURN: void
    */
    public void GenerateTxt(Invoice? invoice)
    {
        if (invoice == null)
        {
            _loggerModel.LogError("Invoice is null.");
            return;
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

     //   var invoicePath = config["Invoice:InvoicePath"];
        var invoicePath = "C:\\Users\\Yafet\\OneDrive\\Desktop\\TMS\\bin\\Debug\\net6.0-windows\\Invoice";

        if (!string.IsNullOrEmpty(invoicePath))
        {
            var filePath = Path.Combine(invoicePath, $"Invoice_{invoice.InvoiceId}.txt");

            try
            {
                // Ensure the directory exists before writing the file
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());

                using (var writer = new StreamWriter(filePath))
                {
                    // Header
                    writer.WriteLine($"Invoice ID: {invoice.InvoiceId}");
                    writer.WriteLine($"Date: {DateTime.Now}");
                    
                    writer.WriteLine();

                    // Order details
                    writer.WriteLine("Order details:");
                    writer.WriteLine($"Order ID: {invoice.OrderId}");
                    writer.WriteLine($"Customer ID: {invoice.CustomerId}");
                    writer.WriteLine($"Customer Name: {invoice.CustomerName}");
                    writer.WriteLine($"Order Origin: {invoice.Origin}");
                    writer.WriteLine($"Order Destination: {invoice.Destination}");
                    writer.WriteLine($"Order Quantity: {invoice.Quantity}");
                    writer.WriteLine($"Date Completed: {invoice.DateCompleted}");
                   
                    writer.WriteLine();
                    
                    // Billing details
                    writer.WriteLine("Billing Details:");
                    writer.WriteLine($"Amount: {invoice.Amount:C}");
                    writer.WriteLine($"Job Type: {invoice.JobType}");
                    writer.WriteLine($"Van Type: {invoice.JobType}");


                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Error here");
                _loggerModel.LogException($"Error creating text file: {e.Message}");
            }
        }
    }

}
