using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DAL;
using System.Configuration; 

namespace eSGBIZ.Controllers
{
    public class MixDesignController : BaseController
    {
        [Authorize]
        public ActionResult MixDesignCal()
        {
            ViewBag.Title = "MixDesignCal";
            return View();
        }
        [HttpPost]
        public ActionResult MixDesignCal(VM_Mix_Design_Entry Entry)
        {
           try
            {
                List<MIX_DESIGN_DATA_Dtl> dtl = new DAL_MIX_DESIGN().GET_LAB_TEST_SCHEDULE_DTLS();
                if (dtl.Count > 0)
                {
                    var firstItem = dtl[0];

                    // Redirect to MixDesign_Entry with parameters
                    return RedirectToAction("MixDesign_Entry", new
                    {
                        TestCode = firstItem.TEST_CODE,
                        GradeName = firstItem.Grade_Name,
                        CustName = firstItem.custName
                    });
                }
                else
                {
                  
                     return RedirectToAction("");
                }
            }
            catch (Exception ex)
            {

                Danger(string.Format("<b>Exception occurred.</b> {0}", ex.Message), true);
            }

            return RedirectToAction("MixDesign_Entry");
        }


        public ActionResult _MixDesignCal_Dtl()
        {

            List<MIX_DESIGN_DATA_Dtl> dtl = new DAL_MIX_DESIGN().GET_LAB_TEST_SCHEDULE_DTLS();
            VM_Mix_Design_Entry SCTEST = new VM_Mix_Design_Entry();
            SCTEST.MIX_DESIGN_DATA_Dtl_List = dtl;

            
            return PartialView("_MixDesignCal_Dtl", SCTEST);
        }



        [Authorize]
        public ActionResult MixDesign_Entry(string TestCode, string GradeName, string CustName)
        {
            ViewBag.Header = "Mix Design Entry";
            VM_Mix_Design_Entry _mixDesign = new VM_Mix_Design_Entry();
            try
            {
                _mixDesign.Sc_Ref_No = TestCode;
                _mixDesign.Client_Name = CustName;


                List<GradeMaster> gradeList = new DAL_Common().GetGradeList();
                _mixDesign.Grade_List = new SelectList(gradeList, "Grade_Id", "Grade_Name");

                GradeMaster selectedGrade = gradeList.FirstOrDefault(g => g.Grade_Name == GradeName);
                if (selectedGrade != null)
                {
                    _mixDesign.Grade_Id = selectedGrade.Grade_Id;
                }
                else
                {

                    _mixDesign.Grade_Id = 0;
                }

                List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
                _mixDesign.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");

                //List<MIX_DESIGN_DATA_Dtl> dtl = new DAL_MIX_DESIGN().GET_LAB_TEST_SCHEDULE_DTLS();
                //_mixDesign.MIX_DESIGN_DATA_Dtl_List = dtl;

            }
            catch (Exception ex)
            {
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }
            return View(_mixDesign);
        }

        //[Authorize]
        //public ActionResult MixDesign_Entry()
        //{
        //    ViewBag.Header = "Mix Design Entry";
        //    VM_Mix_Design_Entry _mixDesign = new VM_Mix_Design_Entry();

         
        //    List<GradeMaster> gradeList = new DAL_Common().GetGradeList();
        //    _mixDesign.Grade_List = new SelectList(gradeList, "Grade_Id", "Grade_Name");

        //    List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
        //    _mixDesign.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");


        //    return View(_mixDesign);
        //}

      



        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("MixDesign_Entry")]
        public ActionResult MixDesign_Entry_Save(VM_Mix_Design_Entry _mixDesign)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_MIX_DESIGN().INSERT_MIX_DESIGN(emp.Employee_Code, _mixDesign, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>Mix design inserted successfully. Sample No. : </b> <b style='color:red'> " + objMst.CODE + "</b>"), true);
                        return RedirectToAction("MixDesign_Entry", "MixDesign");
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }

            List<GradeMaster> gradeList = new DAL_Common().GetGradeList();
            _mixDesign.Grade_List = new SelectList(gradeList, "Grade_Id", "Grade_Name");

            List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
            _mixDesign.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");


            return View(_mixDesign);
        }


        [Authorize]
        public ActionResult MixDesignList()
        {
            ViewBag.Header = "Mix Design";
            VM_MIX_DESIGN_LIST _mixDesign = new VM_MIX_DESIGN_LIST();

            List<GradeMaster> gradeList = new DAL_Common().GetGradeList();
            _mixDesign.Grade_List = new SelectList(gradeList, "Grade_Id", "Grade_Name"); 

            return View(_mixDesign);

        }

        public ActionResult _MixDesign_List()
        {
            return PartialView("_MixDesign_List");
        }

        [HttpPost]
        public ActionResult _MixDesign_Data_List(string grade, string tCode, DateTime fDate, DateTime tDate)
        {
            // Server Side Processing
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int totalRow = 0;

            VM_MIX_DESIGN_LIST _mixDesign = new VM_MIX_DESIGN_LIST();
            List<MIX_DESIGN_DATA_LIST> MixDesignDatas = new List<MIX_DESIGN_DATA_LIST>();
            try
            {
                if (string.IsNullOrEmpty( grade))
                {
                    grade = "0"; 
                }
                _mixDesign.Grade_Id = Convert.ToInt32( grade);
                _mixDesign.TM_CODE = tCode;
                _mixDesign.From_DT = fDate;
                _mixDesign.To_DT = tDate;

                MixDesignDatas = new DAL_MIX_DESIGN().Select_MixDesign_Data_List(_mixDesign);

                totalRow = MixDesignDatas.Count();

            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error : _CNs_Data_List ", ex.Message);
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }

            if (!string.IsNullOrEmpty(searchValue)) // Filter Operation
            {
                MixDesignDatas = MixDesignDatas.
                    Where(x => x.TM_CODE.ToLower().Contains(searchValue.ToLower()) || x.Grade_Name.ToLower().Contains(searchValue.ToLower())
                        || x.Client_Name.ToLower().Contains(searchValue.ToLower())
                         ).ToList<MIX_DESIGN_DATA_LIST>();




            }
            int totalRowFilter = MixDesignDatas.Count();
            // sorting
            //sievesDatas = sievesDatas.OrderBy(sortColumnName + " " + sortDirection).ToList<SIEVE_ANALYSIS_DATA_LIST>();

            // Paging
            if (length == -1)
            {
                length = totalRow;
            }
            MixDesignDatas = MixDesignDatas.Skip(start).Take(length).ToList<MIX_DESIGN_DATA_LIST>();

            var jsonResult = Json(new { data = MixDesignDatas, draw = Request["draw"], recordsTotal = totalRow, recordsFiltered = totalRowFilter }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult MixDesign_View(decimal TM_ID)
        {
            MIX_DESIGN_VIEW _objMixDesign = new MIX_DESIGN_VIEW();
            _objMixDesign = new DAL_MIX_DESIGN().VIEW_MIX_DESIGN(TM_ID);
            return PartialView("MixDesign_View", _objMixDesign);

        }

        public FileResult ShowDocument(string FilePath)
        {
            string DMS_Path = ConfigurationManager.AppSettings["DMSPATH"].ToString();
            string directoryPath = DMS_Path + "MIX_DESIGN\\" + FilePath;

            //return File(Server.MapPath("~/Files/") + FilePath, GetMimeType(FilePath));
            return File(directoryPath, GetMimeType(FilePath));
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

    }
}
