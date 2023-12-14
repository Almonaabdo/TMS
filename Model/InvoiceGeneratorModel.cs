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
    /// <param name="order"></param>
    /// <param name="rates"></param>
    /// <param name="customer"></param>
    /// <returns></returns>
    public Invoice GenerateInvoice(JoinedOrder order)
    {
        
        int orderId = 0 ;
        double tripCost = 0;
        int customerId = 0;
        try
        { 
            if (order != null)
            {
                orderId = order.OrderId;
                tripCost = order.TripCost;
                customerId = order.CustomerId;
                MessageBox.Show("order nottt nulllll");
            }
            else
            {
                MessageBox.Show("order is nulllll");
            }
            var invoice = new Invoice               // Create a new invoice
            {
                OrderId = orderId,
                Amount = tripCost,
                InvoiceDate = DateTime.Today,
                CustomerId = customerId
            };

            MessageBox.Show("Createddddd unvoice");
            _dbContext.InvoiceDetails?.Add(invoice); // Add invoice to db
            _dbContext.SaveChanges();                // Save changes

            MessageBox.Show("Saved unvoice");
            return invoice;                          // Return the create invoice
        }
        catch (Exception ex) 
        {
            MessageBox.Show($"Error {ex.Message}");
            return null;                          // Return the create invoice
        }

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
                writer.WriteLine($"Quantity: {invoice.Quantity}");
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