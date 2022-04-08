﻿using OfficeOpenXml;
using SbdcImpactPredictor.Interfaces;
using SbdcImpactPredictor.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SbdcImpactPredictor.ServiceFacades
{
    public class LoansService : ILoansService
    {
        public List<Loan> GetLoans(int year,string locationName)
        {
            var loans = new List<Loan>();
            string format = "dd/MM/yyyy";
            var d = new DirectoryInfo(@"..\SbdcImpactPredictor\Files");
            var files = d.GetFiles("*.xlsx");
            foreach (var file in files)
            {
                var fileName = file.FullName;
                using var package = new ExcelPackage(file);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;

                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    var loan = new Loan
                    {
                        UniqueIdentifier = workSheet.Cells[rowIterator, 1].Value.ToString(),
                        CreditType = (CreditType)Enum.Parse(typeof(CreditType), workSheet.Cells[rowIterator, 2].Value.ToString(), true),
                        AmountApplied = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value),
                        AmountApproved = Convert.ToInt32(workSheet.Cells[rowIterator, 4].Value),
                        ActionTakenDate = DateTime.ParseExact(workSheet.Cells[rowIterator, 5].Value.ToString(), format, new CultureInfo("en-US"), DateTimeStyles.None),
                        Status = (LoanStatus)Enum.Parse(typeof(LoanStatus), workSheet.Cells[rowIterator, 6].Value.ToString(), true),
                        DenialInformation = (DenialReason)Enum.Parse(typeof(DenialReason), workSheet.Cells[rowIterator, 7].Value.ToString(), true),
                        MinorityOwned = (bool)workSheet.Cells[rowIterator, 8].Value,
                        Gender = workSheet.Cells[rowIterator, 9].Value.ToString(),
                        Latitude = Convert.ToDecimal(workSheet.Cells[rowIterator, 10].Value),
                        Longitude = Convert.ToDecimal(workSheet.Cells[rowIterator, 11].Value),
                        State = workSheet.Cells[rowIterator, 12].Value.ToString(),
                        InterestRate = Convert.ToDecimal(workSheet.Cells[rowIterator, 13].Value)
                    };
                    loans.Add(loan);
                }
            }
            loans = loans.Where(l => l.State.ToLower() == locationName.ToLower() && l.ActionTakenDate.Year == year).ToList();
            return loans;
        }
    }
}