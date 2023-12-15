using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog.Fluent;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class InvoiceGeneratorModel
{
    #region Fields

    private readonly ConfigService _configService = new();

    #endregion

    #region Generate Invoice

    /*
     * METHOD NAME: CreateInvoice
     * DESCRIPTION: Method to create an invoice and save to db
     * RETURN: The created invoice in a list
     */
    public List<Invoice>? CreateInvoice(int orderId, int customerId, double tripCost, string? destination, string? origin)
    {
        try
        {
            if (DbContextSingleton.Instance.Orders != null)
            {
                var order = DbContextSingleton.Instance.Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (DbContextSingleton.Instance.Customers != null)
                {
                    var customer = DbContextSingleton.Instance.Customers.FirstOrDefault(c => c.CustomerId == customerId);

                    if (order != null && customer != null)
                    {
                        // Create the invoice
                        var invoice = new Invoice
                        {
                            InvoiceDate = DateTime.Now,
                            OrderId = order.OrderId,
                            Amount = tripCost,
                            DateCompleted = order.DateCompleted,
                            CustomerName = customer.Name,
                            JobType = order.JobType,
                            VanType = order.VanType,
                            Quantity = order.Quantity,
                            Destination = destination,
                            Origin = origin,
                            CustomerId = customerId
                        };
                        // Add to db and save changes
                        DbContextSingleton.Instance.Invoice?.Add(invoice);
                        DbContextSingleton.Instance.SaveChanges();

                        // Return the newly created invoice
                        return new List<Invoice> { invoice };
                    }
                }
            }

            LoggerModel.Instance.LogError($"Order or Customer not found for orderId: {orderId} and customerId: {customerId}");
        }
        catch (InvalidOperationException ex)
        {
            LoggerModel.Instance.LogException($"Error creating invoice: {ex.Message}");
        }
        catch (Exception ex)
        {
            LoggerModel.Instance.LogException($"Error creating invoice: {ex}");
        }

        // Return empty list if invoice generation fails
        return new List<Invoice>();
    }

    #endregion

    #region Generate Text File

    /*
     * METHOD NAME: GenerateTxtInvoice
     * DESCRIPTION: Method to create a text file for the invoice with all the info
     * RETURN: void
     */
    public void GenerateTxtInvoice(List<Invoice>? invoices)
    {
        if (invoices == null)
        {
            LoggerModel.Instance.LogError("Invoice is null.");
            return;
        }

        var invoicePath = _configService.GetInvoicePath();

        if (!string.IsNullOrEmpty(invoicePath))
            try
            {
                foreach (var invoice in invoices)
                {
                    var filePath = Path.Combine(invoicePath, $"Invoice_{invoice.InvoiceId}.txt");

                    // Ensure the directory exists before writing the file
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath) ??
                                              throw new InvalidOperationException());

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
                        writer.WriteLine($"Van Type: {invoice.VanType}");
                    }
                }
            }
            catch (Exception e)
            {
                LoggerModel.Instance.LogException($"Error creating text file: {e.Message}");
            }
    }

    #endregion

    #region Generate Summary Content

    /*
     * METHOD NAME: GenerateSummary
     * DESCRIPTION: Method to generate the summary content
     * RETURN: void
     */
    private string GenerateSummary(List<Invoice> invoices)
    {
        var summaryContent = "Summary Report\n";
        // Iterate through all the invoices and generate the summary
        foreach (var invoice in invoices)
        {
            // Header
            summaryContent += $"Invoice ID: {invoice.InvoiceId}\n";
            summaryContent += $"Date: {DateTime.Now}\n";

            summaryContent += "\n";

            // Order details
            summaryContent += "Order details:\n";
            summaryContent += $"Order ID: {invoice.OrderId}\n";
            summaryContent += $"Customer ID: {invoice.CustomerId}\n";
            summaryContent += $"Customer Name: {invoice.CustomerName}\n";
            summaryContent += $"Order Origin: {invoice.Origin}\n";
            summaryContent += $"Order Destination: {invoice.Destination}\n";
            summaryContent += $"Order Quantity: {invoice.Quantity}\n";
            summaryContent += $"Date Completed: {invoice.DateCompleted}\n";

            summaryContent += "\n";

            // Billing details
            summaryContent += "Billing Details:\n";
            summaryContent += $"Amount: {invoice.Amount:C}\n";
            summaryContent += $"Job Type: {invoice.JobType}\n";
            summaryContent += $"Van Type: {invoice.VanType}\n";

            summaryContent += "\n" + "---------------------------" + "\n";
        }

        return summaryContent; // Return the content
    }

    #endregion

    #region All Time Summary

    /*
     * METHOD NAME: ReportAllTime
     * DESCRIPTION: Method to create a summary report for all time period
     *
     * RETURN: void
     */
    public void ReportAllTime()
    {
        var invoices = DbContextSingleton.Instance.Invoice?.ToList() ?? new List<Invoice>(); // Retrieve all invoices

        if (invoices.Any())
        {
            // Grab filepath from json file
            var invoicePath = _configService.GetInvoicePath();
            if (invoicePath != null)
            {
                var filePath = Path.Combine(invoicePath, "SummaryReport1.txt"); // Construct file path
                var summary = GenerateSummary(invoices); // Generate the summary content
                try
                {
                    Directory.CreateDirectory(invoicePath); // Create directory if it doest exist
                    File.AppendAllText(filePath, summary); // Append summary to file
                }
                catch (Exception e)
                { 
                    LoggerModel.Instance.LogException($"{e.Message}"); // Log exception
                }
            }
        }
    }

    #endregion

    #region Two Week Summary

    /*
     * METHOD NAME: ReportTwoWeeks
     * DESCRIPTION: Method to create a summary report for two week period
     *
     * RETURN: void
     */
    public void ReportTwoWeeks()
    {
        try
        {
            // Grab all invoices for a two week period
            var invoices = DbContextSingleton.Instance.Invoice?.Where(i => i.InvoiceDate >= DateTime.Now.AddDays(-14))?.ToList() ??
                           new List<Invoice>();

            if (invoices.Any())
            {
                var invoicePath = _configService.GetInvoicePath(); // Call method to get invoice path

                var summary = GenerateSummary(invoices); // Generate summary content

                if (invoicePath != null) // If path is valid
                {
                    var fileName = Path.Combine(invoicePath, $"TwoWeekSummaryReport_{DateTime.UtcNow}.txt"); // Construct filename
                    try
                    {
                        Directory.CreateDirectory(invoicePath);
                        File.AppendAllText(fileName, summary);
                    }
                    catch (Exception e)
                    {
                        LoggerModel.Instance.LogException($"{e.Message}");
                    }
                }
            }
        }
        catch (Exception e)
        {
           LoggerModel.Instance.LogException($"{e.Message}");
        }
    }

    #endregion
}