using System;
using System.IO;
using System.Windows;
using NLog;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class InvoiceGeneratorModel
{
    private readonly TmsDbContext _dbContext = DbContextSingleton.Instance;

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
    public Invoice GenerateInvoice(string orderId, string tripCost, string customerId)
    {
        try
        {
            if (int.TryParse(orderId, out var idOrder) && double.TryParse(tripCost, out var cost) && int.TryParse(customerId, out var idCustomer))
            {
                Console.WriteLine($"Parsed values: idOrder={idOrder}, cost={cost}, idCustomer={idCustomer}");
                var newInvoice = new Invoice
                {
                    OrderId = idOrder,
                    CustomerId = idCustomer,
                    Amount = cost,
                    InvoiceDate = DateTime.Now,
                };

                _dbContext.Invoice?.Add(newInvoice);
                Console.WriteLine("Added new invoice");
                _dbContext.SaveChanges();
                Console.WriteLine("Saved changes");
            }
            else
            {
                Console.WriteLine("Not working, its null");
            }
            
               
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating invoice: {ex.Message}");
        }

        return null!;
    }


    /// <summary>
    /// Method to create a text file invoice with all details included
    /// </summary>
    /// <param name="invoice">The invoice to create file for</param>
    public void GenerateTxt(Invoice? invoice)
    {
        if (invoice == null)
        {
            Console.WriteLine("Invalid invoice");
        }

        string filePath = $"Invoice_{invoice.InvoiceId}.txt"; // Generate file path based on invoice id

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))                            
            {
                // Header
                writer.WriteLine($"Invoice ID: {invoice.InvoiceId}");
                writer.WriteLine($"Date: {DateTime.Now}");
                writer.WriteLine();

                // Order details
                writer.WriteLine("Order details:");
                writer.WriteLine($"Order ID: {invoice.OrderId}");
                writer.WriteLine($"Customer ID: {invoice.CustomerId}");
                writer.WriteLine();

                // Billing details
                writer.WriteLine("Billing Details:");
                writer.WriteLine($"Amount: {invoice.Amount:C}");

                // Just for confirmation, 
                MessageBox.Show($"TXT document saved to: {filePath}");
            }
        }
        catch (Exception e)
        {
            LoggerModel.LogException( $"Error creating text file {e.Message}");
        }
    }
}