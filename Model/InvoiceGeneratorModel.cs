using System;
using System.IO;
using NLog;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class InvoiceGeneratorModel
{
    private readonly TmsDbContext _dbContext;

    public InvoiceGeneratorModel()
    {
        _dbContext = new TmsDbContext();
    }

    /// <summary>
    /// Method to create an invoice
    /// </summary>
    /// <param name="order"></param>
    /// <param name="rates"></param>
    /// <param name="customer"></param>
    /// <returns></returns>
    public Invoice GenerateInvoice(Order order, Rate rates, Customer customer)
    {
        decimal Amount = 100; // Call method to get invoice amount here, just a place holder for now

        var invoice = new Invoice  // Create a new invoice
        {
            OrderId = order.OrderId,
            RateId = rates.RateId,
            Amount = Amount,
            InvoiceDate = DateTime.Today,
            CustomerId = customer.CustomerId
        };

        _dbContext.InvoiceDetails?.Add(invoice); // Add invoice to db
        _dbContext.SaveChanges(); // Save changes

        return invoice; // Return the create invoice
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
                writer.WriteLine($"Rate ID: {invoice.RateId}");
                writer.WriteLine($"Quantity: {invoice.Quantity}");
                writer.WriteLine($"Amount: {invoice.Amount:C}");

                // Just for confirmation, 
                Console.WriteLine($"TXT document saved to: {filePath}");
            }
        }
        catch (Exception e)
        {
            LoggerModel.LogException( $"Error creating text file {e.Message}");
        }
    }


}