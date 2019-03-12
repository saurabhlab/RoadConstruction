using RP.ViewModel.Common;
using RP.ViewModel.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.IServices
{
   public interface IInwardOutwardService
    {
        ResponseViewModel SaveInwardOutwardData(InwardOutwardVM inwardOutwardVM);
        ResponseViewModel SaveWorkGenerateData(WorkVM workVM);
 ResponseViewModel UpdateWorkCompleteData(WorkVM workVM);
ResponseViewModel WorkRegenrate(IdViewModel idViewModel);
        ResponseViewModel GetMaterialReportsList(ReportFilterVM model);
 ResponseViewModel GetAdminMaterialReportsList(ReportFilterVM model);

        ResponseViewModel GetWorkReportList(IdViewModel idViewModel);
ResponseViewModel GetWorkDetailsById(IdViewModel idViewModel);
        //ResponseViewModel GetAdminWorkReportList(IdViewModel idViewModel);
        ResponseViewModel GetAdminWorkReportList(ReportFilterVM model);
        ResponseViewModel GetWorkReportListForManager(ReportFilterVM model);
ResponseViewModel AssignWork(AssignWorkVM assignWorkVM);

        ResponseViewModel GenrateWorkReportPdf(PdfReportVM pdfReportVM);
  ResponseViewModel DownloadMaterialReportPdf(PdfReportVM pdfReportVM);
 ResponseViewModel DeleteExistingPdf(IdViewModel idViewModel);
        ResponseViewModel GetWorkCaseNo(WorkVM workVM);
 ResponseViewModel DeleteMaterialById(IdViewModel idViewModel);
 ResponseViewModel DeleteWorkById(IdViewModel idViewModel);

    }
}
