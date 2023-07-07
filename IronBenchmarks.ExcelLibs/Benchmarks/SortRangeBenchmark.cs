﻿using Aspose.Cells;
using BenchmarkDotNet.Attributes;
using ClosedXML.Excel;
using IronBenchmarks.ExcelLibs.Benchmarks.Bases;
using IronBenchmarks.ExcelLibs.Config;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;

namespace IronBenchmarks.ExcelLibs.Benchmarks
{
    [ShortRunJob]
    [MemoryDiagnoser]
    [Config(typeof(ExcelConfig))]
    public class SortRangeBenchmark : BenchmarkBase
    {
        private readonly string _sortRangeFileName = "SortRangeFiles\\SortRange.xlsx";
        private IronXL.WorkSheet _ixlSortRange;
        private IronXLOld.WorkSheet _ixlOldSortRange;
        private Workbook _asposeSortRangeWb;
        private DataSorter _asposeSorter;
        private Cells _asposeCells;
        private readonly CellArea _asposeCellArea = new CellArea
        {
            StartRow = 0,
            StartColumn = 0,
            EndRow = 999,
            EndColumn = 1
        };
        private IXLRange _closedXmlSortRange;
        private XSSFSheet _npoiSortRange;
        private ExcelPackage _epplusSortRangeWb;
        private ExcelWorksheet _epplusSortRangeSheet;
        private ExcelRange _epplusSortRange;

        [IterationSetup]
        public void IterationSetup()
        {
            _asposeSortRangeWb = new Workbook(_sortRangeFileName);
            _asposeCells = _asposeSortRangeWb.Worksheets[0].Cells;
            _asposeSorter = GetAsposeSorter();

            _ixlSortRange = IronXL.WorkBook.Load(_sortRangeFileName).DefaultWorkSheet;

            _ixlOldSortRange = IronXLOld.WorkBook.Load(_sortRangeFileName).DefaultWorkSheet;

            _npoiSortRange = (XSSFSheet)new XSSFWorkbook(_sortRangeFileName).GetSheetAt(0);
            _ = _npoiSortRange.GetRow(0);

            _closedXmlSortRange = new XLWorkbook(_sortRangeFileName).Worksheet("ToSort").Range("A1:B1000");

            _epplusSortRangeWb = new ExcelPackage(_sortRangeFileName);
            _epplusSortRangeSheet = _epplusSortRangeWb.Workbook.Worksheets[0];
            _epplusSortRange = _epplusSortRangeSheet.Cells["A1:B1000"];
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _ixlSortRange.WorkBook.Close();
            _ixlSortRange = null;

            _ixlOldSortRange.WorkBook.Close();
            _ixlOldSortRange = null;

            _asposeSortRangeWb.Dispose();
            _asposeCells.Dispose();
            _asposeSortRangeWb = null;
            _asposeCells = null;
            _asposeSorter = null;

            _closedXmlSortRange.Worksheet.Workbook.Dispose();
            _closedXmlSortRange = null;

            _epplusSortRange.Dispose();
            _epplusSortRange = null;
            _epplusSortRangeSheet.Dispose();
            _epplusSortRangeSheet = null;
            _epplusSortRangeWb.Dispose();
            _epplusSortRangeWb = null;

        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Aspose")]
        public override void Aspose()
        {
            _ = _asposeSorter.Sort(_asposeCells, _asposeCellArea);
        }

        [Benchmark]
        [BenchmarkCategory("ClosedXml")]
        public override void ClosedXml()
        {
            _ = _closedXmlSortRange.Sort(1);
        }

        [Benchmark]
        [BenchmarkCategory("Epplus")]
        public override void Epplus()
        {
            _epplusSortRange.Sort(x => x.SortBy.Column(0));
        }

        [Benchmark]
        [BenchmarkCategory("IronXl")]
        public override void IronXl()
        {
            _ = _ixlSortRange.SortByColumn(0, IronXL.SortOrder.Ascending);
        }

        [Benchmark]
        [BenchmarkCategory("Iron_XlOld")]
        public override void Iron_XlOld()
        {
            _ = _ixlOldSortRange.SortByColumn(0, IronXLOld.SortOrder.Ascending);
        }

        [Benchmark]
        [BenchmarkCategory("Npoi")]
        public override void Npoi()
        {
            throw new NotImplementedException();
        }

        private DataSorter GetAsposeSorter()
        {
            DataSorter sorter = _asposeSortRangeWb.DataSorter;
            sorter.Order1 = SortOrder.Ascending;
            sorter.Key1 = 0;

            return sorter;
        }
    }
}
