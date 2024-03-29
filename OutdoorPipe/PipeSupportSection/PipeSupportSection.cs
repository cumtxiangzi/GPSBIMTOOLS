using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Interop;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class PipeSupportSection : IExternalCommand
    {
        public static PipeSupportSectionForm mainfrm;
        public Result Execute(ExternalCommandData commandData, ref string messages, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Selection sel = uidoc.Selection;

                mainfrm = new PipeSupportSectionForm();
                IntPtr rvtPtr = Process.GetCurrentProcess().MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(mainfrm);
                helper.Owner = rvtPtr;
                mainfrm.Show();

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            return Result.Succeeded;
        }
    }
    public class ExecuteEventPipeSupportSection : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            try
            {
                UIDocument uidoc = app.ActiveUIDocument;
                Document doc = app.ActiveUIDocument.Document;
                Selection sel = app.ActiveUIDocument.Selection;

                View view = uidoc.ActiveView;
                if (view is ViewDrafting)
                {
                    CreatPipeSupportSection(doc, sel);
                }
                else
                {
                    TaskDialog.Show("警告", "请在绘制视图中进行操作");
                    PipeSupportSection.mainfrm.Show();
                }

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                //MessageBox.Show(e.ToString());
            }
        }
        public string GetName()
        {
            return "管道支架剖面";
        }
        public void CreatPipeSupportSection(Document doc, Selection sel)
        {
            XYZ pickpoint = sel.PickPoint("请选择插入点");

            FamilyInstance typeA_Section = null;
            FamilyInstance typeB_Section = null;
            FamilyInstance typeC_Section = null;
            FamilyInstance typeC_Section_UpThree = null;
            FamilyInstance typeD_Section = null;
            FamilyInstance typeE_Section = null;
            FamilyInstance typeF_Section = null;

            TransactionGroup tg = new TransactionGroup(doc, "创建管道支架详图");
            tg.Start();

            using (Transaction trans = new Transaction(doc, "载入支架详图族"))
            {
                trans.Start();

                if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "A型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "B型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "C型支架");
                    DetailDrawingFamilyLoad(doc, "C型支架三四层");
                }
                else if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "D型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "E型支架");
                }
                else if (PipeSupportSection.mainfrm.TypeF_Button.IsChecked == true)
                {
                    DetailDrawingFamilyLoad(doc, "F型支架");
                }

                DetailDrawingTitleLoad(doc, "图名");
                DetailDrawingTitleLoad(doc, "支架剖面管道标注");

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "布置支架详图族"))
            {
                trans.Start();

                if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
                {
                    FamilySymbol typeA_SectionSymbol = null;
                    typeA_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeA_Button.Content.ToString());
                    typeA_SectionSymbol.Activate();
                    typeA_Section = doc.Create.NewFamilyInstance(pickpoint, typeA_SectionSymbol, doc.ActiveView);
                    TypeA_ModifyParameter(typeA_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
                {
                    FamilySymbol typeB_SectionSymbol = null;
                    typeB_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeB_Button.Content.ToString());
                    typeB_SectionSymbol.Activate();
                    typeB_Section = doc.Create.NewFamilyInstance(pickpoint, typeB_SectionSymbol, doc.ActiveView);
                    TypeB_ModifyParameter(typeB_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true || PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        FamilySymbol typeC_SectionSymbol = null;
                        typeC_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeC_Button.Content.ToString());
                        typeC_SectionSymbol.Activate();
                        typeC_Section = doc.Create.NewFamilyInstance(pickpoint, typeC_SectionSymbol, doc.ActiveView);
                        TypeC_ModifyParameter(typeC_Section);
                    }
                    else
                    {
                        FamilySymbol typeC_Section_UpThreeSymbol = null;
                        typeC_Section_UpThreeSymbol = PipeSupportSectionSymbol(doc, "C型支架三四层");
                        typeC_Section_UpThreeSymbol.Activate();
                        typeC_Section_UpThree = doc.Create.NewFamilyInstance(pickpoint, typeC_Section_UpThreeSymbol, doc.ActiveView);
                        TypeC_UpThreeModifyParameter(typeC_Section_UpThree);
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
                {
                    FamilySymbol typeD_SectionSymbol = null;
                    typeD_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeD_Button.Content.ToString());
                    typeD_SectionSymbol.Activate();
                    typeD_Section = doc.Create.NewFamilyInstance(pickpoint, typeD_SectionSymbol, doc.ActiveView);
                    TypeD_ModifyParameter(typeD_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
                {
                    FamilySymbol typeE_SectionSymbol = null;
                    typeE_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeE_Button.Content.ToString());
                    typeE_SectionSymbol.Activate();
                    typeE_Section = doc.Create.NewFamilyInstance(pickpoint, typeE_SectionSymbol, doc.ActiveView);
                    TypeE_ModifyParameter(typeE_Section);
                }
                else if (PipeSupportSection.mainfrm.TypeF_Button.IsChecked == true)
                {
                    FamilySymbol typeF_SectionSymbol = null;
                    typeF_SectionSymbol = PipeSupportSectionSymbol(doc, PipeSupportSection.mainfrm.TypeF_Button.Content.ToString());
                    typeF_SectionSymbol.Activate();
                    typeF_Section = doc.Create.NewFamilyInstance(pickpoint, typeF_SectionSymbol, doc.ActiveView);
                    TypeF_ModifyParameter(typeF_Section);
                }

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "创建尺寸标注"))
            {
                trans.Start();

                if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
                {
                    TypeA_CreatDimensionX(doc, typeA_Section, pickpoint);
                    TypeA_CreatDimensionY(doc, typeA_Section, pickpoint);
                }
                else if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
                {
                    TypeB_CreatDimensionX(doc, typeB_Section, pickpoint);
                    TypeB_CreatDimensionY(doc, typeB_Section, pickpoint);
                }
                else if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        double height = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "一层管道");
                        XYZ dimPosition = new XYZ(pickpoint.X, pickpoint.Y + height + 100 / 304.8, pickpoint.Z);
                        TypeC_CreatDimensionX(doc, typeC_Section, "一层支架边界线", "一层管道中心线", dimPosition);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        double height1 = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "一层管道");
                        // typeC_Section.LookupParameter("一层支架净高H1").SetValueString((height1 * 304.8 + 150).ToString());
                        typeC_Section.LookupParameter("一层支架净高H1").SetValueString(GetFloorHeight(Convert.ToInt32(height1 * 304.8)).ToString());

                        XYZ dimPosition1 = new XYZ(pickpoint.X, pickpoint.Y - 250 / 304.8, pickpoint.Z);
                        TypeC_CreatDimensionX(doc, typeC_Section, "一层支架边界线", "一层管道中心线", dimPosition1);

                        double height2 = MaximumDiameter(doc, typeC_Section, doc.ActiveView, "二层管道");
                        XYZ dimPosition2 = new XYZ(pickpoint.X, pickpoint.Y + height1 + height2 + 600 / 304.8, pickpoint.Z); //此处有BUG，小直径管道尺寸线位置不准确
                        TypeC_CreatDimensionX(doc, typeC_Section, "二层支架边界线", "二层管道中心线", dimPosition2);

                        double width = 0;
                        double width1 = typeC_Section.LookupParameter("一层支架长L1").AsDouble();
                        double width2 = typeC_Section.LookupParameter("二层支架长L2").AsDouble();
                        if (width1 > width2)
                        {
                            width = width1;
                        }
                        else
                        {
                            width = width2;
                        }
                        XYZ dimPosition3 = new XYZ(pickpoint.X + width + 100 / 304.8, pickpoint.Y, pickpoint.Z);
                        TypeC_CreatDimensionY(doc, typeC_Section, "一层支架边界线", dimPosition3);
                    }
                    else if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true || PipeSupportSection.mainfrm.FourFloor.IsChecked == true)
                    {
                        TypeC_CreatDimensionX(doc, typeC_Section_UpThree, pickpoint);
                        TypeC_CreatDimensionY(doc, typeC_Section_UpThree, pickpoint);
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
                {
                    TypeD_CreatDimensionX(doc, typeD_Section, pickpoint);
                    TypeD_CreatDimensionY(doc, typeD_Section, pickpoint);
                }
                else if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
                {
                    TypeE_CreatDimensionX(doc, typeE_Section, pickpoint);
                    TypeE_CreatDimensionY(doc, typeE_Section, pickpoint);
                }
                else if (PipeSupportSection.mainfrm.TypeF_Button.IsChecked == true)
                {
                    TypeF_CreatDimensionX(doc, typeF_Section, pickpoint);
                }

                trans.Commit();
            }
            using (Transaction trans = new Transaction(doc, "创建图名及管道信息标注"))
            {
                trans.Start();

                if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
                {
                    CreatTitle(doc, pickpoint, typeA_Section);

                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        TypeA_CreatOneFloorPipeNote(doc, typeA_Section);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeA_CreatTwoFloorPipeNote(doc, typeA_Section);
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
                {
                    CreatTitle(doc, pickpoint, typeB_Section);

                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        TypeB_CreatOneFloorPipeNote(doc, typeB_Section);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeB_CreatTwoFloorPipeNote(doc, typeB_Section);
                    }
                    else if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
                    {
                        TypeB_CreatThreeFloorPipeNote(doc, typeB_Section);
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
                {
                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true || PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        CreatTitle(doc, pickpoint, typeC_Section);
                    }
                    else
                    {
                        CreatTitle(doc, pickpoint, typeC_Section_UpThree);
                    }

                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        TypeC_CreatOneFloorPipeNote(doc, typeC_Section);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeC_CreatOneFloorPipeNote(doc, typeC_Section);
                        TypeC_CreatTwoFloorPipeNote(doc, typeC_Section);
                    }
                    else if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true || PipeSupportSection.mainfrm.FourFloor.IsChecked == true)
                    {
                        TypeC_CreatUpThreeFloorPipeNote(doc, typeC_Section_UpThree);
                        typeC_Section_UpThree.LookupParameter("支柱名称").Set(PipeSupportSection.mainfrm.WorkshopGridName.Text);
                        typeC_Section_UpThree.LookupParameter("标高值").Set(PipeSupportSection.mainfrm.LevelValue.Text);
                        typeC_Section_UpThree.LookupParameter("支柱名称下划线长度").SetValueString((PipeSupportSection.mainfrm.WorkshopGridName.Text.Length * 3.57 + 20).ToString());
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
                {
                    CreatTitle(doc, pickpoint, typeD_Section);

                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        TypeD_CreatOneFloorPipeNote(doc, typeD_Section);
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeD_CreatTwoFloorPipeNote(doc, typeD_Section);
                    }
                }
                else if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
                {
                    CreatTitle(doc, pickpoint, typeE_Section);                  

                    if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
                    {
                        TypeE_CreatOneFloorPipeNote(doc, typeE_Section);                     
                    }
                    else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                    {
                        TypeE_CreatTwoFloorPipeNote(doc, typeE_Section);
                    }

                    string baseLength = typeE_Section.LookupParameter("基础长度").AsValueString();
                    typeE_Section.LookupParameter("预埋钢板文字说明").Set("预埋" + baseLength + "X200X10mm钢板");
                }
                else if (PipeSupportSection.mainfrm.TypeF_Button.IsChecked == true)
                {
                    TypeF_CreatOneFloorPipeNote(doc, typeF_Section);
                }

                trans.Commit();
            }

            tg.Assimilate();

            PipeSupportSection.mainfrm.SupportCode.Text = PipeSupportSection.mainfrm.name.Insert(1, PipeSupportSection.mainfrm.clickNum.ToString());
            PipeSupportSection.mainfrm.Show();
        }

        #region 支架参数修改
        public void TypeA_ModifyParameter(FamilyInstance sectionInstance) //A型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(0);
                    sectionInstance.LookupParameter("一层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("三层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层净高H").SetValueString("-150");
                }
                else
                {
                    sectionInstance.LookupParameter("一层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                }

                TypeA_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(0);
                    sectionInstance.LookupParameter("二层净高H").SetValueString("-150");
                }
                else
                {
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(1);
                }

                TypeA_TwoFloorPipeSection(sectionInstance);
            }

        }
        public void TypeA_OneFloorPipeSection(FamilyInstance sectionInstance) //A型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("支架净长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 * 2).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                         PipeDistance(oneFloorRightPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item3 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                       PipeDistance(oneFloorRightPipe2_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                }
            }
            else
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("支架净长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2).ToString());
                    sectionInstance.LookupParameter("二层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                         PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLenght.ToString());
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                       PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("支架净长度L").SetValueString(totalLength.ToString());
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }
            }
        }
        public void TypeA_TwoFloorPipeSection(FamilyInstance sectionInstance) //A型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                int oneFloorLenght = 1000;
                int twoFloorLenght = 1000;

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                    sectionInstance.LookupParameter("一层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                         PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                       PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2;
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                    twoFloorLenght = totalLength;
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                         PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                        - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                         PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                        - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                       PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                        PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    twoFloorLenght = totalLength;
                }

                if (twoFloorLenght > oneFloorLenght)
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(twoFloorLenght.ToString());
                }
                else
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(oneFloorLenght.ToString());
                }
            }
            else
            {
                int oneFloorLenght = 1000;
                int twoFloorLenght = 1000;

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                    sectionInstance.LookupParameter("一层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                         PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    oneFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                       PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    oneFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("一层净高H").SetValueString(floorHeight.ToString());
                }

                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2;
                    sectionInstance.LookupParameter("二层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(twoFloorLeftPipe1_Size)).ToString());
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                    twoFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());

                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                         PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                        - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());

                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                         PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                        - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    twoFloorLenght = totalLenght;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());

                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                       PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                        PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    twoFloorLenght = totalLength;
                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层净高H").SetValueString(floorHeight.ToString());
                }

                if (twoFloorLenght > oneFloorLenght)
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(twoFloorLenght.ToString());
                }
                else
                {
                    sectionInstance.LookupParameter("支架净长度L").SetValueString(oneFloorLenght.ToString());
                }
            }
        }
        public void TypeB_ModifyParameter(FamilyInstance sectionInstance) //B型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("三层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(1);
                }

                TypeB_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.CableTray.IsChecked == false)
                {
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                }
                else
                {
                    sectionInstance.LookupParameter("三层横撑可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("电缆桥架可见性").Set(1);
                }

                TypeB_TwoFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                TypeB_ThreeFloorPipeSection(sectionInstance);
            }
        }
        public void TypeB_OneFloorPipeSection(FamilyInstance sectionInstance) //B型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 * 2).ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                         PipeDistance(oneFloorRightPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3
                        - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item3 +
                         PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3
                        - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                       PipeDistance(oneFloorRightPipe2_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 -
                        PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 -
                        PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }
            }
            else
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorRightPipe1_Size)).ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                      + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                     + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize);
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
            }

        }
        public void TypeB_TwoFloorPipeSection(FamilyInstance sectionInstance) //B型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            if (!cableTray_Check)
            {
                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2).ToString());
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                         PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                        - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);

                    int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                         PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                         + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                        - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                            PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }
                    else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                    {
                        MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                            , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString("0");
                        sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                        sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                           PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    }

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                       PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                       + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                        PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                        PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
                }

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)) + 100).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(oneFloorRightPipe1_Size)) + 100).ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                      + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                     + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
            }
            else
            {
                //二层管道参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(twoFloorLeftPipe1_Size)) + 0).ToString());
                }
                if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3 +
                        PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(twoFloorRightPipe1_Size)) + 0).ToString());
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 0;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                      + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 0;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                        + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 0;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                     + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 0;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                        + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                     + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 0;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                        + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                     + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 0;
                    sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
                }

                //一层管道参数修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)) + 100).ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                        PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(oneFloorRightPipe1_Size)) + 100).ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                      + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                        + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                     + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                    sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                    sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                    sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                    sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                        + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                     + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                    List<int> pipeSizeList = new List<int>();
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                    pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                    pipeSizeList.Sort();
                    int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                    int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                    sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
                }
            }

        }
        public void TypeB_ThreeFloorPipeSection(FamilyInstance sectionInstance) //B型支架修改三层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorLeftPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorLeftPipe2_Size = PipeSupportSection.mainfrm.ThreeFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorRightPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorRightPipe2_Size = PipeSupportSection.mainfrm.ThreeFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool threeFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorLeftPipe1.IsChecked;
            bool threeFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorLeftPipe2.IsChecked;
            bool threeFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorRightPipe1.IsChecked;
            bool threeFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorRightPipe2.IsChecked;

            //三层管道参数修改
            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && !threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("三层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                sectionInstance.LookupParameter("三层横撑长度L").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item3 * 2).ToString());
            }

            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(threeFloorLeftPipe1_Size).Item3 + PipeDistance(threeFloorRightPipe1_Size).Item3 +
                    PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorLeftPipe1_Size).Item3).ToString());
                sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorRightPipe1_Size).Item3).ToString());

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
            }

            if (threeFloorLeftPipe1_Check && threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(threeFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(threeFloorLeftPipe2_Size).Item3 +
                     PipeDistance(threeFloorRightPipe1_Size).Item3 + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3 - PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3
                    - PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorRightPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3 - PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
            }

            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(threeFloorRightPipe2_Size);
                sectionInstance.LookupParameter("三层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(threeFloorRightPipe2_Size).Item3 +
                     PipeDistance(threeFloorLeftPipe1_Size).Item3 + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3 - PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3
                    - PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 - PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLenght / 2 - PipeDistance(threeFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3 - PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(threeFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLenght.ToString());
            }

            if (threeFloorLeftPipe1_Check && threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && threeFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("三层左侧管道D1").SetValueString(threeFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("三层左侧管道D2").SetValueString(threeFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("三层右侧管道D1").SetValueString(threeFloorRightPipe1_Size);
                sectionInstance.LookupParameter("三层右侧管道D2").SetValueString(threeFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(threeFloorLeftPipe2_Size).Item3 +
                   PipeDistance(threeFloorRightPipe2_Size).Item3 + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(threeFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("三层左侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item3 -
                    PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("三层右侧管道1与支柱中心间距").SetValueString((totalLength / 2 - PipeDistance(threeFloorRightPipe2_Size).Item3 -
                    PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 - PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("三层左侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(threeFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("三层右侧管道1与管道2间距").SetValueString((PipeDistance(threeFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(threeFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("三层横撑长度L").SetValueString(totalLength.ToString());
            }


            //二层管道参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());

                sectionInstance.LookupParameter("二层与三层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(twoFloorLeftPipe1_Size)) + 100).ToString());
            }
            if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3 +
                    PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                sectionInstance.LookupParameter("二层与三层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(twoFloorRightPipe1_Size)) + 100).ToString());
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
            }
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                  + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
            }
            if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                    + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                 + PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
            }
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe2_Size).Item3
                    + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item3
                 + PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                sectionInstance.LookupParameter("二层左侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与支柱壁间距").SetValueString(PipeDistance(twoFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("二层左侧横撑长度L").SetValueString((PipeDistance(twoFloorLeftPipe2_Size).Item3
                    + PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧横撑长度L").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item3
                 + PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("二层与三层间距H").SetValueString(floorHeight.ToString());
            }

            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                sectionInstance.LookupParameter("一层与二层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)) + 100).ToString());
            }
            if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3 +
                    PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                sectionInstance.LookupParameter("一层与二层间距H").SetValueString((GetFloorHeight(Convert.ToInt32(oneFloorRightPipe1_Size)) + 100).ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
            }
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                  + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());

            }
            if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层左侧横撑可见性").Set(0);

                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                    + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                 + PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
            }
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());

                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe2_Size).Item3
                    + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item3
                 + PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                sectionInstance.LookupParameter("一层左侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorLeftPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与支柱壁间距").SetValueString(PipeDistance(oneFloorRightPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("一层左侧横撑长度L").SetValueString((PipeDistance(oneFloorLeftPipe2_Size).Item3
                    + PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧横撑长度L").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item3
                 + PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize) + 100;
                sectionInstance.LookupParameter("一层与二层间距H").SetValueString(floorHeight.ToString());
            }

        }
        public void TypeC_ModifyParameter(FamilyInstance sectionInstance) //C型支架修改参数
        {
            sectionInstance.LookupParameter("支柱名称").Set(PipeSupportSection.mainfrm.WorkshopGridName.Text);
            sectionInstance.LookupParameter("支架底部标高").Set(PipeSupportSection.mainfrm.LevelValue.Text);
            sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
            sectionInstance.LookupParameter("支柱名称下划线长度L").SetValueString((PipeSupportSection.mainfrm.WorkshopGridName.Text.Length * 3.57 + 10).ToString());

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("二层支架横撑显隐").Set(0);
                sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                sectionInstance.LookupParameter("二层管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                TypeC_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                TypeC_TwoFloorPipeSection(sectionInstance);
            }
        }
        public void TypeC_UpThreeModifyParameter(FamilyInstance sectionInstance) //C型支架修改参数
        {
            if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("四层可见性").Set(0);
                TypeC_ThreeFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.FourFloor.IsChecked == true)
            {
                TypeC_FourFloorPipeSection(sectionInstance);
            }

        }
        public void TypeC_OneFloorPipeSection(FamilyInstance sectionInstance) //C型支架修改一层管道参数
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                if (HaveBrace(oneFloorPipe1_Size))
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(1);
                }
                else
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(0);
                }
                sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                if (HaveBrace(oneFloorPipe1_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 250).ToString());
                }
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size))
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(1);
                }
                else
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(0);
                }
                sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                             PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 250).ToString());
                }
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size))
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(1);
                }
                else
                {
                    sectionInstance.LookupParameter("斜撑显隐").Set(0);
                }
                sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                             PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size))
                {
                    sectionInstance.LookupParameter("斜撑净宽B").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                              PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 250).ToString());
                }
            }
        }
        public void TypeC_TwoFloorPipeSection(FamilyInstance sectionInstance) //C型支架修改二层管道参数
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe2_Size = PipeSupportSection.mainfrm.TwoFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe3_Size = PipeSupportSection.mainfrm.TwoFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe4_Size = PipeSupportSection.mainfrm.TwoFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool twoFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe1.IsChecked;
            bool twoFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe2.IsChecked;
            bool twoFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe3.IsChecked;
            bool twoFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe4.IsChecked;

            if (twoFloorPipe1_Check && !twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());

                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());

                    if (HaveBrace(oneFloorPipe1_Size) || HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size) || HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size) || HaveBrace(twoFloorPipe1_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());

                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道直径D2").SetValueString(twoFloorPipe2_Size);
                sectionInstance.LookupParameter("二层管道3可见性").Set(0);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层管道1与管道2中心间距").SetValueString((PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2).ToString());

                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());

                    if (HaveBrace(oneFloorPipe1_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());

                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item3).ToString());
                    }
                }

            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("二层管道直径D1").SetValueString(twoFloorPipe1_Size);
                sectionInstance.LookupParameter("二层管道直径D2").SetValueString(twoFloorPipe2_Size);
                sectionInstance.LookupParameter("二层管道直径D3").SetValueString(twoFloorPipe3_Size);
                sectionInstance.LookupParameter("二层管道4可见性").Set(0);
                sectionInstance.LookupParameter("二层管道1距墙净距L1").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("二层管道1与管道2中心间距").SetValueString((PipeDistance(twoFloorPipe1_Size).Item2 / 2 + PipeDistance(twoFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层管道2与管道3中心间距").SetValueString((PipeDistance(twoFloorPipe2_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2).ToString());

                if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道2可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());

                    if (HaveBrace(oneFloorPipe1_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道3可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());
                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                    }
                }

                if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
                {
                    sectionInstance.LookupParameter("一层管道直径D1").SetValueString(oneFloorPipe1_Size);
                    sectionInstance.LookupParameter("一层管道直径D2").SetValueString(oneFloorPipe2_Size);
                    sectionInstance.LookupParameter("一层管道直径D3").SetValueString(oneFloorPipe3_Size);
                    sectionInstance.LookupParameter("一层管道4可见性").Set(0);
                    sectionInstance.LookupParameter("一层管道1距墙净距L1").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                    sectionInstance.LookupParameter("一层管道1与管道2中心间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层管道2与管道3中心间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                    if (HaveBrace(oneFloorPipe1_Size, oneFloorPipe2_Size, oneFloorPipe3_Size) || HaveBrace(twoFloorPipe1_Size, twoFloorPipe2_Size, twoFloorPipe3_Size))
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(1);
                        sectionInstance.LookupParameter("斜撑显隐").Set(1);

                        double width1 = PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3;
                        double width2 = PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3;
                        double width = width1 > width2 ? width1 : width2;

                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((width + 100).ToString());
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((width - 0).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString((width - 150).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((width + 100).ToString());

                    }
                    else
                    {
                        sectionInstance.LookupParameter("二层支架竖撑显隐").Set(0);
                        sectionInstance.LookupParameter("斜撑显隐").Set(0);
                        sectionInstance.LookupParameter("支架底部膨胀螺栓定位H1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3 - 100).ToString());
                        sectionInstance.LookupParameter("斜撑净宽B").SetValueString("400");
                        sectionInstance.LookupParameter("一层支架长L1").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(oneFloorPipe2_Size).Item2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item3).ToString());
                        sectionInstance.LookupParameter("二层支架长L2").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item2 / 2 +
                                                PipeDistance(twoFloorPipe2_Size).Item2 + PipeDistance(twoFloorPipe3_Size).Item2 / 2 + PipeDistance(twoFloorPipe3_Size).Item3).ToString());
                    }
                }
            }
        }
        public void TypeC_ThreeFloorPipeSection(FamilyInstance sectionInstance)//C型支架修改三层管道参数
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");

            sectionInstance.LookupParameter("一层管道D1").SetValueString(oneFloorPipe1_Size);
            sectionInstance.LookupParameter("二层管道D1").SetValueString(twoFloorPipe1_Size);
            sectionInstance.LookupParameter("三层管道D1").SetValueString(threeFloorPipe1_Size);

            sectionInstance.LookupParameter("一层管道与墙间距").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
            sectionInstance.LookupParameter("二层管道与墙间距").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());
            sectionInstance.LookupParameter("三层管道与墙间距").SetValueString(PipeDistance(threeFloorPipe1_Size).Item1.ToString());

            sectionInstance.LookupParameter("一层长度").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
            sectionInstance.LookupParameter("二层长度").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
            sectionInstance.LookupParameter("三层长度").SetValueString((PipeDistance(threeFloorPipe1_Size).Item1 + PipeDistance(threeFloorPipe1_Size).Item3).ToString());

            sectionInstance.LookupParameter("一层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorPipe1_Size)).ToString());
            sectionInstance.LookupParameter("二层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(twoFloorPipe1_Size)).ToString());
        }
        public void TypeC_FourFloorPipeSection(FamilyInstance sectionInstance)//C型支架修改四层管道参数
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string threeFloorPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string fourFloorPipe1_Size = PipeSupportSection.mainfrm.FourFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");

            sectionInstance.LookupParameter("一层管道D1").SetValueString(oneFloorPipe1_Size);
            sectionInstance.LookupParameter("二层管道D1").SetValueString(twoFloorPipe1_Size);
            sectionInstance.LookupParameter("三层管道D1").SetValueString(threeFloorPipe1_Size);
            sectionInstance.LookupParameter("四层管道D1").SetValueString(fourFloorPipe1_Size);

            sectionInstance.LookupParameter("一层管道与墙间距").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
            sectionInstance.LookupParameter("二层管道与墙间距").SetValueString(PipeDistance(twoFloorPipe1_Size).Item1.ToString());
            sectionInstance.LookupParameter("三层管道与墙间距").SetValueString(PipeDistance(threeFloorPipe1_Size).Item1.ToString());
            sectionInstance.LookupParameter("四层管道与墙间距").SetValueString(PipeDistance(fourFloorPipe1_Size).Item1.ToString());

            sectionInstance.LookupParameter("一层长度").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe1_Size).Item3).ToString());
            sectionInstance.LookupParameter("二层长度").SetValueString((PipeDistance(twoFloorPipe1_Size).Item1 + PipeDistance(twoFloorPipe1_Size).Item3).ToString());
            sectionInstance.LookupParameter("三层长度").SetValueString((PipeDistance(threeFloorPipe1_Size).Item1 + PipeDistance(threeFloorPipe1_Size).Item3).ToString());
            sectionInstance.LookupParameter("四层长度").SetValueString((PipeDistance(fourFloorPipe1_Size).Item1 + PipeDistance(fourFloorPipe1_Size).Item3).ToString());

            sectionInstance.LookupParameter("一层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorPipe1_Size)).ToString());
            sectionInstance.LookupParameter("二层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(twoFloorPipe1_Size)).ToString());
            sectionInstance.LookupParameter("三层净高H").SetValueString(GetFloorHeight(Convert.ToInt32(threeFloorPipe1_Size)).ToString());
        }
        public void TypeD_ModifyParameter(FamilyInstance sectionInstance) //D型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("二层支架可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层高度H").SetValueString("0");

                TypeD_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                TypeD_TwoFloorPipeSection(sectionInstance);
            }

        }
        public void TypeD_OneFloorPipeSection(FamilyInstance sectionInstance) //D型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;

            int oneFloorLenght = 1000;
            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("一层高度H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                     PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                    - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                     PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                    - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                   PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                    PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            sectionInstance.LookupParameter("支架长度L").SetValueString(oneFloorLenght.ToString());
        }
        public void TypeD_TwoFloorPipeSection(FamilyInstance sectionInstance) //D型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;

            int oneFloorLenght = 1000;
            int twoFloorLenght = 1000;

            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("一层高度H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                     PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                    - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                     PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                    - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                   PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                    PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("一层高度H").SetValueString(floorHeight.ToString());
            }

            //二层管道参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("二层高度H").SetValueString(GetFloorHeight(Convert.ToInt32(twoFloorLeftPipe1_Size)).ToString());
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorRightPipe1_Size).Item1 +
                    PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());

                twoFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item1 +
                     PipeDistance(twoFloorRightPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1
                    - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                twoFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item1 +
                     PipeDistance(twoFloorLeftPipe1_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1
                    - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                twoFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item1 +
                   PipeDistance(twoFloorRightPipe2_Size).Item1 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item1 -
                    PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item1 -
                    PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                twoFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(twoFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层高度H").SetValueString(floorHeight.ToString());
            }

            if (twoFloorLenght > oneFloorLenght)
            {
                sectionInstance.LookupParameter("支架长度L").SetValueString(twoFloorLenght.ToString());
            }
            else
            {
                sectionInstance.LookupParameter("支架长度L").SetValueString(oneFloorLenght.ToString());
            }


        }
        public void TypeE_ModifyParameter(FamilyInstance sectionInstance) //E型支架修改参数
        {
            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                sectionInstance.LookupParameter("二层支架可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                TypeE_OneFloorPipeSection(sectionInstance);
            }
            else if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                TypeE_TwoFloorPipeSection(sectionInstance);
            }

        }
        public void TypeE_OneFloorPipeSection(FamilyInstance sectionInstance) //E型支架修改一层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;

            int oneFloorLenght = 1000;
            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item3 * 2;
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorRightPipe1_Size).Item3 +
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());

                oneFloorLenght = totalLength;
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                     PipeDistance(oneFloorRightPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3
                    - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item3 +
                     PipeDistance(oneFloorLeftPipe1_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3
                    - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item3 +
                   PipeDistance(oneFloorRightPipe2_Size).Item3 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item3 -
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 -
                    PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                oneFloorLenght = totalLength;
            }

            sectionInstance.LookupParameter("支架长度L").SetValueString(oneFloorLenght.ToString());
        }
        public void TypeE_TwoFloorPipeSection(FamilyInstance sectionInstance) //E型支架修改二层管道参数
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;

            int oneFloorLenght = 1000;
            int twoFloorLenght = 1000;

            //一层管道参数修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                oneFloorLenght = PipeDistance(oneFloorLeftPipe1_Size).Item1 * 2;
                sectionInstance.LookupParameter("二层支架净高度H").SetValueString(GetFloorHeight(Convert.ToInt32(oneFloorLeftPipe1_Size)).ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorRightPipe1_Size).Item1 +
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层支架净高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                     PipeDistance(oneFloorRightPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1
                    - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 - PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层左侧管道1、一层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层支架净高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);
                sectionInstance.LookupParameter("一层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(oneFloorRightPipe2_Size).Item1 +
                     PipeDistance(oneFloorLeftPipe1_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item3 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1
                    - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(oneFloorLeftPipe1_Size).Item1).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 - PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保一层右侧管道1、一层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                oneFloorLenght = totalLenght;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层支架净高度H").SetValueString(floorHeight.ToString());
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("一层左侧管道D1").SetValueString(oneFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("一层左侧管道D2").SetValueString(oneFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("一层右侧管道D1").SetValueString(oneFloorRightPipe1_Size);
                sectionInstance.LookupParameter("一层右侧管道D2").SetValueString(oneFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(oneFloorLeftPipe2_Size).Item1 +
                   PipeDistance(oneFloorRightPipe2_Size).Item1 + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("一层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item1 -
                    PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(oneFloorRightPipe2_Size).Item1 -
                    PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 - PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层左侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(oneFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("一层右侧管道1与管道2间距").SetValueString((PipeDistance(oneFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(oneFloorRightPipe2_Size).Item2 / 2).ToString());

                oneFloorLenght = totalLength;
                List<int> pipeSizeList = new List<int>();
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorLeftPipe2_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe1_Size));
                pipeSizeList.Add(Convert.ToInt32(oneFloorRightPipe2_Size));
                pipeSizeList.Sort();
                int maxPipeSize = pipeSizeList[pipeSizeList.Count - 1];
                int floorHeight = GetFloorHeight(maxPipeSize);
                sectionInstance.LookupParameter("二层支架净高度H").SetValueString(floorHeight.ToString());
            }

            //二层管道参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道1可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                twoFloorLenght = PipeDistance(twoFloorLeftPipe1_Size).Item3 * 2;
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                int totalLength = GetPipeDistance10((PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorRightPipe1_Size).Item3 +
                    PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2));
                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());

                twoFloorLenght = totalLength;
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                     PipeDistance(twoFloorRightPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3
                    - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 - PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保二层左侧管道1、二层右侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                }

                twoFloorLenght = totalLenght;
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);
                sectionInstance.LookupParameter("二层左侧管道2可见性").Set(0);

                int totalLenght = GetPipeDistance10(PipeDistance(twoFloorRightPipe2_Size).Item3 +
                     PipeDistance(twoFloorLeftPipe1_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                     + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) > 0)
                {
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3
                    - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLenght / 2 - PipeDistance(twoFloorLeftPipe1_Size).Item3).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                        PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }
                else if ((totalLenght / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 - PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 -
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2) < 0)
                {
                    MessageBox.Show("请确保二层右侧管道1、二层左侧管道1和管道2勾选！" + "\n" + "请手动删除生成的支架详图！"
                        , "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString("0");
                    sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe1_Size).Item2 / 2).ToString());
                    sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                       PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                }

                twoFloorLenght = totalLenght;
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                sectionInstance.LookupParameter("二层左侧管道D1").SetValueString(twoFloorLeftPipe1_Size);
                sectionInstance.LookupParameter("二层左侧管道D2").SetValueString(twoFloorLeftPipe2_Size);
                sectionInstance.LookupParameter("二层右侧管道D1").SetValueString(twoFloorRightPipe1_Size);
                sectionInstance.LookupParameter("二层右侧管道D2").SetValueString(twoFloorRightPipe2_Size);

                int totalLength = GetPipeDistance10(PipeDistance(twoFloorLeftPipe2_Size).Item3 +
                   PipeDistance(twoFloorRightPipe2_Size).Item3 + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2
                   + PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 + PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2 + PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                   PipeDistance(twoFloorRightPipe2_Size).Item2 / 2);

                sectionInstance.LookupParameter("二层左侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item3 -
                    PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 - PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与中心间距").SetValueString((totalLength / 2 - PipeDistance(twoFloorRightPipe2_Size).Item3 -
                    PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 - PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层左侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorLeftPipe1_Size).Item2 / 2 +
                   PipeDistance(twoFloorLeftPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("二层右侧管道1与管道2间距").SetValueString((PipeDistance(twoFloorRightPipe1_Size).Item2 / 2 +
                    PipeDistance(twoFloorRightPipe2_Size).Item2 / 2).ToString());

                twoFloorLenght = totalLength;
            }

            if (twoFloorLenght > oneFloorLenght)
            {
                sectionInstance.LookupParameter("支架长度L").SetValueString(twoFloorLenght.ToString());
            }
            else
            {
                sectionInstance.LookupParameter("支架长度L").SetValueString((oneFloorLenght + 200).ToString());
            }

        }
        public void TypeF_ModifyParameter(FamilyInstance sectionInstance) //F型支架修改参数
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString().Replace("DN", "");
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString().Replace("DN", "");

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            //一层管道参数修改
            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("管道1直径").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("管道2可见性").Set(0);
                sectionInstance.LookupParameter("管道3可见性").Set(0);
                sectionInstance.LookupParameter("管道4可见性").Set(0);

                sectionInstance.LookupParameter("管道1与支架左端间距").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("支架长度L").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 * 2).ToString());
            }
            if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("管道1直径").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("管道2直径").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("管道3可见性").Set(0);
                sectionInstance.LookupParameter("管道4可见性").Set(0);

                sectionInstance.LookupParameter("管道1与支架左端间距").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("管道1与管道2间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("支架长度L").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe2_Size).Item1 +
                   PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
            }
            if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("管道1直径").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("管道2直径").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("管道3直径").SetValueString(oneFloorPipe3_Size);
                sectionInstance.LookupParameter("管道4可见性").Set(0);

                sectionInstance.LookupParameter("管道1与支架左端间距").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("管道1与管道2间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("管道2与管道3间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("支架长度L").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe3_Size).Item1 +
                   PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2 +
                   PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());
            }
            if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && oneFloorPipe4_Check)
            {
                sectionInstance.LookupParameter("管道1直径").SetValueString(oneFloorPipe1_Size);
                sectionInstance.LookupParameter("管道2直径").SetValueString(oneFloorPipe2_Size);
                sectionInstance.LookupParameter("管道3直径").SetValueString(oneFloorPipe3_Size);
                sectionInstance.LookupParameter("管道4直径").SetValueString(oneFloorPipe4_Size);

                sectionInstance.LookupParameter("管道1与支架左端间距").SetValueString(PipeDistance(oneFloorPipe1_Size).Item1.ToString());
                sectionInstance.LookupParameter("管道1与管道2间距").SetValueString((PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("管道2与管道3间距").SetValueString((PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2).ToString());
                sectionInstance.LookupParameter("管道3与管道4间距").SetValueString((PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe4_Size).Item2 / 2).ToString());

                sectionInstance.LookupParameter("支架长度L").SetValueString((PipeDistance(oneFloorPipe1_Size).Item1 + PipeDistance(oneFloorPipe4_Size).Item1 +
                   PipeDistance(oneFloorPipe1_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2 + PipeDistance(oneFloorPipe2_Size).Item2 / 2 +
                   PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe3_Size).Item2 / 2 + PipeDistance(oneFloorPipe4_Size).Item2 / 2).ToString());
            }

        }
        #endregion

        #region 创建支架剖面全部管道信息标注       
        public void TypeA_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //A型支架创建一层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double length = typeC_Section.LookupParameter("二层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

        }
        public void TypeA_CreatTwoFloorPipeNote(Document doc, FamilyInstance typeC_Section) //A型支架创建二层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString();
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString();
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString();
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Abb.SelectedItem.ToString();
            string twoFloorRightPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Abb.SelectedItem.ToString();
            string twoFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Abb.SelectedItem.ToString();


            //一层管道修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double length = typeC_Section.LookupParameter("一层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            //二层参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double length = typeC_Section.LookupParameter("二层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }
        }
        public void TypeB_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //B型支架创建一层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();

            if (!cableTray_Check)
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double length = typeC_Section.LookupParameter("三层左侧管道D1").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("三层右侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与管道2间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("三层右侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                }

                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("三层右侧管道1与管道2间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与管道2间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("三层右侧管道1与管道2间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }
            }
            else
            {
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double length = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionList.ElementAt(0), -900);
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double length = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol rightPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }
            }
        }
        public void TypeB_CreatTwoFloorPipeNote(Document doc, FamilyInstance typeC_Section) //B型支架创建二层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString();
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString();
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString();
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Abb.SelectedItem.ToString();
            string twoFloorRightPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Abb.SelectedItem.ToString();
            string twoFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Abb.SelectedItem.ToString();

            if (!cableTray_Check)
            {
                //一层管道修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double length = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionList.ElementAt(0), -900);
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double length = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol rightPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                //二层参数修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double length = typeC_Section.LookupParameter("三层左侧管道D1").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                    CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("三层右侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与管道2间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("三层右侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                }

                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与支柱中心间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("三层右侧管道1与管道2间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与管道2间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("三层右侧管道1与管道2间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
                }
            }
            else
            {
                //一层管道修改
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                    double length = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionList.ElementAt(0), -900);
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                    double length = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol rightPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }
                if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
                }
                if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
                }

                //二层管道修改
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double length = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionList.ElementAt(0), -900);
                }
                if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double length = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                    AnnotationSymbol rightPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);
                }
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
                }
                if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
                }
                if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                    XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                    AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
                }

                if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
                {
                    List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                    double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                    MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                    XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                    MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                    List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                    double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                    XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                    XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                    CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
                }
            }
        }
        public void TypeB_CreatThreeFloorPipeNote(Document doc, FamilyInstance typeC_Section) //B型支架创建三层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString();
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString();
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString();
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString();
            string threeFloorLeftPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorLeftPipe1_Size.SelectedItem.ToString();
            string threeFloorLeftPipe2_Size = PipeSupportSection.mainfrm.ThreeFloorLeftPipe2_Size.SelectedItem.ToString();
            string threeFloorRightPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorRightPipe1_Size.SelectedItem.ToString();
            string threeFloorRightPipe2_Size = PipeSupportSection.mainfrm.ThreeFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;
            bool threeFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorLeftPipe1.IsChecked;
            bool threeFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorLeftPipe2.IsChecked;
            bool threeFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorRightPipe1.IsChecked;
            bool threeFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.ThreeFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Abb.SelectedItem.ToString();
            string twoFloorRightPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Abb.SelectedItem.ToString();
            string twoFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Abb.SelectedItem.ToString();
            string threeFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.ThreeFloorLeftPipe1_Abb.SelectedItem.ToString();
            string threeFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.ThreeFloorLeftPipe2_Abb.SelectedItem.ToString();
            string threeFloorRightPipe1_Abb = PipeSupportSection.mainfrm.ThreeFloorRightPipe1_Abb.SelectedItem.ToString();
            string threeFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.ThreeFloorRightPipe2_Abb.SelectedItem.ToString();

            //一层管道修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double length = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionList.ElementAt(0), -900);
            }
            if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double length = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                AnnotationSymbol rightPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);
            }
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }
            if (!oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            //二层管道修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double length = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionList.ElementAt(0), -900);
            }
            if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double length = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                AnnotationSymbol rightPipeNote = CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);
            }
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
            }
            if (!twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
            }
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与支柱壁间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }

            //三层参数修改
            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && !threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                double length = typeC_Section.LookupParameter("三层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, threeFloorLeftPipe1_Abb + "-" + threeFloorLeftPipe1_Size, PipeWeight(threeFloorLeftPipe1_Size));
            }

            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与支柱中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, threeFloorLeftPipe1_Abb + "-" + threeFloorLeftPipe1_Size, PipeWeight(threeFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("三层右侧管道1与支柱中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, threeFloorRightPipe1_Abb + "-" + threeFloorRightPipe1_Size, PipeWeight(threeFloorRightPipe1_Size));
            }

            if (threeFloorLeftPipe1_Check && threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && !threeFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, threeFloorLeftPipe1_Abb + "-" + threeFloorLeftPipe1_Size, PipeWeight(threeFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, threeFloorLeftPipe2_Abb + "-" + threeFloorLeftPipe2_Size, PipeWeight(threeFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("三层右侧管道1与支柱中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, threeFloorRightPipe1_Abb + "-" + threeFloorRightPipe1_Size, PipeWeight(threeFloorRightPipe1_Size));

            }

            if (threeFloorLeftPipe1_Check && !threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && threeFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与支柱中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, threeFloorLeftPipe1_Abb + "-" + threeFloorLeftPipe1_Size, PipeWeight(threeFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("三层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, threeFloorRightPipe1_Abb + "-" + threeFloorRightPipe1_Size, PipeWeight(threeFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, threeFloorRigthPipe2_Abb + "-" + threeFloorRightPipe2_Size, PipeWeight(threeFloorRightPipe2_Size));
            }

            if (threeFloorLeftPipe1_Check && threeFloorLeftPipe2_Check && threeFloorRightPipe1_Check && threeFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("三层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft1, typeC_Section, threeFloorLeftPipe1_Abb + "-" + threeFloorLeftPipe1_Size, PipeWeight(threeFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft2, typeC_Section, threeFloorLeftPipe2_Abb + "-" + threeFloorLeftPipe2_Size, PipeWeight(threeFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("三层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, threeFloorRightPipe1_Abb + "-" + threeFloorRightPipe1_Size, PipeWeight(threeFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, threeFloorRigthPipe2_Abb + "-" + threeFloorRightPipe2_Size, PipeWeight(threeFloorRightPipe2_Size));
            }

        }
        public void TypeC_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //C型支架创建一层全部管道信息标注
        {

            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString();
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString();
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString();
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString();

            string oneFloorPipe1_Abb = PipeSupportSection.mainfrm.OneFloorPipe1_Abb.SelectedItem.ToString();
            string oneFloorPipe2_Abb = PipeSupportSection.mainfrm.OneFloorPipe2_Abb.SelectedItem.ToString();
            string oneFloorPipe3_Abb = PipeSupportSection.mainfrm.OneFloorPipe3_Abb.SelectedItem.ToString();
            string oneFloorPipe4_Abb = PipeSupportSection.mainfrm.OneFloorPipe4_Abb.SelectedItem.ToString();

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            string oneFloorPipe1_Weight = PipeWeight(oneFloorPipe1_Size);
            string oneFloorPipe2_Weight = PipeWeight(oneFloorPipe2_Size);
            string oneFloorPipe3_Weight = PipeWeight(oneFloorPipe3_Size);
            string oneFloorPipe4_Weight = PipeWeight(oneFloorPipe4_Size);

            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length = typeC_Section.LookupParameter("一层管道1距墙净距L1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length1 = typeC_Section.LookupParameter("一层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("一层管道1与管道2中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y - length2 * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y - length2 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
            }
            else if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length1 = typeC_Section.LookupParameter("一层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("一层管道1与管道2中心间距").AsDouble();
                double length3 = typeC_Section.LookupParameter("一层管道2与管道3中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y - length2 * Math.Tan(60 * Math.PI / 180) - 240 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y - length2 * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                XYZ instancePoint3 = new XYZ(positionList.ElementAt(2).X + length3, positionList.ElementAt(2).Y - length3 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
                CreatPipeNote(doc, positionList.ElementAt(2), instancePoint3, typeC_Section, oneFloorPipe3_Abb + "-" + oneFloorPipe3_Size, PipeWeight(oneFloorPipe3_Size));
            }
        }
        public void TypeC_CreatTwoFloorPipeNote(Document doc, FamilyInstance typeC_Section) //C型支架创建二层全部管道信息标注
        {
            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString();
            string twoFloorPipe2_Size = PipeSupportSection.mainfrm.TwoFloorPipe2_Size.SelectedItem.ToString();
            string twoFloorPipe3_Size = PipeSupportSection.mainfrm.TwoFloorPipe3_Size.SelectedItem.ToString();
            string twoFloorPipe4_Size = PipeSupportSection.mainfrm.TwoFloorPipe4_Size.SelectedItem.ToString();

            string twoFloorPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorPipe1_Abb.SelectedItem.ToString();
            string twoFloorPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorPipe2_Abb.SelectedItem.ToString();
            string twoFloorPipe3_Abb = PipeSupportSection.mainfrm.TwoFloorPipe3_Abb.SelectedItem.ToString();
            string twoFloorPipe4_Abb = PipeSupportSection.mainfrm.TwoFloorPipe4_Abb.SelectedItem.ToString();

            bool twoFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe1.IsChecked;
            bool twoFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe2.IsChecked;
            bool twoFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe3.IsChecked;
            bool twoFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.TwoFloorPipe4.IsChecked;

            string twoFloorPipe1_Weight = PipeWeight(twoFloorPipe1_Size);
            string twoFloorPipe2_Weight = PipeWeight(twoFloorPipe2_Size);
            string twoFloorPipe3_Weight = PipeWeight(twoFloorPipe3_Size);
            string twoFloorPipe4_Weight = PipeWeight(twoFloorPipe4_Size);

            if (twoFloorPipe1_Check && !twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
                double length = typeC_Section.LookupParameter("二层管道1距墙净距L1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && !twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
                double length1 = typeC_Section.LookupParameter("二层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("二层管道1与管道2中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y + length2 * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y + length2 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, twoFloorPipe2_Abb + "-" + twoFloorPipe2_Size, PipeWeight(twoFloorPipe2_Size));
            }
            else if (twoFloorPipe1_Check && twoFloorPipe2_Check && twoFloorPipe3_Check && !twoFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
                double length1 = typeC_Section.LookupParameter("二层管道1距墙净距L1").AsDouble();
                double length2 = typeC_Section.LookupParameter("二层管道1与管道2中心间距").AsDouble();
                double length3 = typeC_Section.LookupParameter("二层管道2与管道3中心间距").AsDouble();
                XYZ instancePoint1 = new XYZ(positionList.ElementAt(0).X + length2, positionList.ElementAt(0).Y + length2 * Math.Tan(60 * Math.PI / 180) + 240 / 304.8, 0);
                XYZ instancePoint2 = new XYZ(positionList.ElementAt(1).X + length2, positionList.ElementAt(1).Y + length2 * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                XYZ instancePoint3 = new XYZ(positionList.ElementAt(2).X + length3, positionList.ElementAt(2).Y + length3 * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint1, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));
                CreatPipeNote(doc, positionList.ElementAt(1), instancePoint2, typeC_Section, twoFloorPipe2_Abb + "-" + twoFloorPipe2_Size, PipeWeight(twoFloorPipe2_Size));
                CreatPipeNote(doc, positionList.ElementAt(2), instancePoint3, typeC_Section, twoFloorPipe3_Abb + "-" + twoFloorPipe3_Size, PipeWeight(twoFloorPipe3_Size));
            }
        }
        public void TypeC_CreatUpThreeFloorPipeNote(Document doc, FamilyInstance typeC_Section) //C型支架创建三四层全部管道信息标注
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString();
            string twoFloorPipe1_Size = PipeSupportSection.mainfrm.TwoFloorPipe1_Size.SelectedItem.ToString();
            string threeFloorPipe1_Size = PipeSupportSection.mainfrm.ThreeFloorPipe1_Size.SelectedItem.ToString();
            string fourFloorPipe1_Size = PipeSupportSection.mainfrm.FourFloorPipe1_Size.SelectedItem.ToString();

            string oneFloorPipe1_Abb = PipeSupportSection.mainfrm.OneFloorPipe1_Abb.SelectedItem.ToString();
            string twoFloorPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorPipe1_Abb.SelectedItem.ToString();
            string threeFloorPipe1_Abb = PipeSupportSection.mainfrm.ThreeFloorPipe1_Abb.SelectedItem.ToString();
            string fourFloorPipe1_Abb = PipeSupportSection.mainfrm.FourFloorPipe1_Abb.SelectedItem.ToString();

            List<XYZ> positionList1 = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
            double length1 = typeC_Section.LookupParameter("一层管道与墙间距").AsDouble();
            XYZ instancePoint1 = new XYZ(positionList1.ElementAt(0).X + length1, positionList1.ElementAt(0).Y + length1 * Math.Tan(60 * Math.PI / 180) - 200 / 304.8, 0);
            CreatPipeNote(doc, positionList1.ElementAt(0), instancePoint1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));

            List<XYZ> positionList2 = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层管道");
            double length2 = typeC_Section.LookupParameter("二层管道与墙间距").AsDouble();
            XYZ instancePoint2 = new XYZ(positionList2.ElementAt(0).X + length2, positionList2.ElementAt(0).Y + length2 * Math.Tan(60 * Math.PI / 180) - 200 / 304.8, 0);
            CreatPipeNote(doc, positionList2.ElementAt(0), instancePoint2, typeC_Section, twoFloorPipe1_Abb + "-" + twoFloorPipe1_Size, PipeWeight(twoFloorPipe1_Size));

            List<XYZ> positionList3 = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "三层管道");
            double length3 = typeC_Section.LookupParameter("三层管道与墙间距").AsDouble();
            XYZ instancePoint3 = new XYZ(positionList3.ElementAt(0).X + length3, positionList3.ElementAt(0).Y + length3 * Math.Tan(60 * Math.PI / 180) - 200 / 304.8, 0);
            CreatPipeNote(doc, positionList3.ElementAt(0), instancePoint3, typeC_Section, threeFloorPipe1_Abb + "-" + threeFloorPipe1_Size, PipeWeight(threeFloorPipe1_Size));

            if (PipeSupportSection.mainfrm.FourFloor.IsChecked == true)
            {
                List<XYZ> positionList4 = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "四层管道");
                double length4 = typeC_Section.LookupParameter("四层管道与墙间距").AsDouble();
                XYZ instancePoint4 = new XYZ(positionList4.ElementAt(0).X + length4, positionList4.ElementAt(0).Y + length4 * Math.Tan(60 * Math.PI / 180) - 200 / 304.8, 0);
                CreatPipeNote(doc, positionList4.ElementAt(0), instancePoint4, typeC_Section, fourFloorPipe1_Abb + "-" + fourFloorPipe1_Size, PipeWeight(fourFloorPipe1_Size));
            }
        }
        public void TypeD_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //D型支架创建一层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double length = typeC_Section.LookupParameter("一层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

        }
        public void TypeD_CreatTwoFloorPipeNote(Document doc, FamilyInstance typeC_Section) //D型支架创建二层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString();
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString();
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString();
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Abb.SelectedItem.ToString();
            string twoFloorRightPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Abb.SelectedItem.ToString();
            string twoFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Abb.SelectedItem.ToString();


            //一层管道修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double length = typeC_Section.LookupParameter("一层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            //二层参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double length = typeC_Section.LookupParameter("二层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }
        }
        public void TypeE_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //E型支架创建一层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double length = typeC_Section.LookupParameter("一层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

        }
        public void TypeE_CreatTwoFloorPipeNote(Document doc, FamilyInstance typeC_Section) //E型支架创建二层全部管道信息标注
        {
            string oneFloorLeftPipe1_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Size.SelectedItem.ToString();
            string oneFloorLeftPipe2_Size = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Size.SelectedItem.ToString();
            string oneFloorRightPipe1_Size = PipeSupportSection.mainfrm.OneFloorRightPipe1_Size.SelectedItem.ToString();
            string oneFloorRightPipe2_Size = PipeSupportSection.mainfrm.OneFloorRightPipe2_Size.SelectedItem.ToString();
            string twoFloorLeftPipe1_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Size.SelectedItem.ToString();
            string twoFloorLeftPipe2_Size = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Size.SelectedItem.ToString();
            string twoFloorRightPipe1_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Size.SelectedItem.ToString();
            string twoFloorRightPipe2_Size = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Size.SelectedItem.ToString();

            bool oneFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe1.IsChecked;
            bool oneFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorLeftPipe2.IsChecked;
            bool oneFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe1.IsChecked;
            bool oneFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorRightPipe2.IsChecked;
            bool twoFloorLeftPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe1.IsChecked;
            bool twoFloorLeftPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorLeftPipe2.IsChecked;
            bool twoFloorRightPipe1_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe1.IsChecked;
            bool twoFloorRightPipe2_Check = (bool)PipeSupportSection.mainfrm.TwoFloorRightPipe2.IsChecked;

            string oneFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe1_Abb.SelectedItem.ToString();
            string oneFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.OneFloorLeftPipe2_Abb.SelectedItem.ToString();
            string oneFloorRightPipe1_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe1_Abb.SelectedItem.ToString();
            string oneFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.OneFloorRightPipe2_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe1_Abb.SelectedItem.ToString();
            string twoFloorLeftPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorLeftPipe2_Abb.SelectedItem.ToString();
            string twoFloorRightPipe1_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe1_Abb.SelectedItem.ToString();
            string twoFloorRigthPipe2_Abb = PipeSupportSection.mainfrm.TwoFloorRightPipe2_Abb.SelectedItem.ToString();

            //一层管道修改
            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && !oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double length = typeC_Section.LookupParameter("一层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y - length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && !oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("一层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y - lengthRight * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

            }

            if (oneFloorLeftPipe1_Check && !oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            if (oneFloorLeftPipe1_Check && oneFloorLeftPipe2_Check && oneFloorRightPipe1_Check && oneFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("一层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorLeftPipe1_Abb + "-" + oneFloorLeftPipe1_Size, PipeWeight(oneFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y - lengthLeft * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorLeftPipe2_Abb + "-" + oneFloorLeftPipe2_Size, PipeWeight(oneFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("一层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, oneFloorRightPipe1_Abb + "-" + oneFloorRightPipe1_Size, PipeWeight(oneFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y - lengthRight1 * Math.Tan(60 * Math.PI / 180) - 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, oneFloorRigthPipe2_Abb + "-" + oneFloorRightPipe2_Size, PipeWeight(oneFloorRightPipe2_Size));
            }

            //二层参数修改
            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && !twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double length = typeC_Section.LookupParameter("二层左侧管道D1").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -900);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && !twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight = typeC_Section.LookupParameter("二层右侧管道1与中心间距").AsDouble();
                XYZ instancePointRight = new XYZ(positionListRight.ElementAt(0).X + lengthRight, positionListRight.ElementAt(0).Y + lengthRight * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

            }

            if (twoFloorLeftPipe1_Check && !twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与中心间距").AsDouble();
                XYZ instancePointLeft = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 120 / 304.8, 0);
                AnnotationSymbol leftPipeNote = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote, positionListLeft.ElementAt(0), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }

            if (twoFloorLeftPipe1_Check && twoFloorLeftPipe2_Check && twoFloorRightPipe1_Check && twoFloorRightPipe2_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层左侧管道");
                double lengthLeft = typeC_Section.LookupParameter("二层左侧管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, twoFloorLeftPipe1_Abb + "-" + twoFloorLeftPipe1_Size, PipeWeight(twoFloorLeftPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, twoFloorLeftPipe2_Abb + "-" + twoFloorLeftPipe2_Size, PipeWeight(twoFloorLeftPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                List<XYZ> positionListRight = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "二层右侧管道");
                double lengthRight1 = typeC_Section.LookupParameter("二层右侧管道1与管道2间距").AsDouble();

                XYZ instancePointRight1 = new XYZ(positionListRight.ElementAt(0).X + lengthRight1, positionListRight.ElementAt(0).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(0), instancePointRight1, typeC_Section, twoFloorRightPipe1_Abb + "-" + twoFloorRightPipe1_Size, PipeWeight(twoFloorRightPipe1_Size));

                XYZ instancePointRight2 = new XYZ(positionListRight.ElementAt(1).X + lengthRight1, positionListRight.ElementAt(1).Y + lengthRight1 * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                CreatPipeNote(doc, positionListRight.ElementAt(1), instancePointRight2, typeC_Section, twoFloorRigthPipe2_Abb + "-" + twoFloorRightPipe2_Size, PipeWeight(twoFloorRightPipe2_Size));
            }
        }
        public void TypeF_CreatOneFloorPipeNote(Document doc, FamilyInstance typeC_Section) //F型支架创建一层全部管道信息标注
        {
            string oneFloorPipe1_Size = PipeSupportSection.mainfrm.OneFloorPipe1_Size.SelectedItem.ToString();
            string oneFloorPipe2_Size = PipeSupportSection.mainfrm.OneFloorPipe2_Size.SelectedItem.ToString();
            string oneFloorPipe3_Size = PipeSupportSection.mainfrm.OneFloorPipe3_Size.SelectedItem.ToString();
            string oneFloorPipe4_Size = PipeSupportSection.mainfrm.OneFloorPipe4_Size.SelectedItem.ToString();

            bool oneFloorPipe1_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe1.IsChecked;
            bool oneFloorPipe2_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe2.IsChecked;
            bool oneFloorPipe3_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe3.IsChecked;
            bool oneFloorPipe4_Check = (bool)PipeSupportSection.mainfrm.OneFloorPipe4.IsChecked;

            string oneFloorPipe1_Abb = PipeSupportSection.mainfrm.OneFloorPipe1_Abb.SelectedItem.ToString();
            string oneFloorPipe2_Abb = PipeSupportSection.mainfrm.OneFloorPipe2_Abb.SelectedItem.ToString();
            string oneFloorPipe3_Abb = PipeSupportSection.mainfrm.OneFloorPipe3_Abb.SelectedItem.ToString();
            string oneFloorPipe4_Abb = PipeSupportSection.mainfrm.OneFloorPipe4_Abb.SelectedItem.ToString();

            if (oneFloorPipe1_Check && !oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionList = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double length = typeC_Section.LookupParameter("管道1与支架左端间距").AsDouble();
                XYZ instancePoint = new XYZ(positionList.ElementAt(0).X + length, positionList.ElementAt(0).Y + length * Math.Tan(60 * Math.PI / 180), 0);
                CreatPipeNote(doc, positionList.ElementAt(0), instancePoint, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
            }

            if (oneFloorPipe1_Check && oneFloorPipe2_Check && !oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double lengthLeft = typeC_Section.LookupParameter("管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

            }

            if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && !oneFloorPipe4_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double lengthLeft = typeC_Section.LookupParameter("管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft3 = new XYZ(positionListLeft.ElementAt(2).X + lengthLeft, positionListLeft.ElementAt(2).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 700 / 304.8, 0);
                AnnotationSymbol leftPipeNote3 = CreatPipeNote(doc, positionListLeft.ElementAt(2), instancePointLeft3, typeC_Section, oneFloorPipe3_Abb + "-" + oneFloorPipe3_Size, PipeWeight(oneFloorPipe3_Size));
                MovePipeNote(doc, leftPipeNote3, positionListLeft.ElementAt(2), -1200);
            }

            if (oneFloorPipe1_Check && oneFloorPipe2_Check && oneFloorPipe3_Check && oneFloorPipe4_Check)
            {
                List<XYZ> positionListLeft = PipeCenterPosition(doc, typeC_Section, doc.ActiveView, "一层管道");
                double lengthLeft = typeC_Section.LookupParameter("管道1与管道2间距").AsDouble();

                XYZ instancePointLeft1 = new XYZ(positionListLeft.ElementAt(0).X + lengthLeft, positionListLeft.ElementAt(0).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 0 / 304.8, 0);
                AnnotationSymbol leftPipeNote1 = CreatPipeNote(doc, positionListLeft.ElementAt(0), instancePointLeft1, typeC_Section, oneFloorPipe1_Abb + "-" + oneFloorPipe1_Size, PipeWeight(oneFloorPipe1_Size));
                MovePipeNote(doc, leftPipeNote1, positionListLeft.ElementAt(0), -1200);

                XYZ instancePointLeft2 = new XYZ(positionListLeft.ElementAt(1).X + lengthLeft, positionListLeft.ElementAt(1).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 350 / 304.8, 0);
                AnnotationSymbol leftPipeNote2 = CreatPipeNote(doc, positionListLeft.ElementAt(1), instancePointLeft2, typeC_Section, oneFloorPipe2_Abb + "-" + oneFloorPipe2_Size, PipeWeight(oneFloorPipe2_Size));
                MovePipeNote(doc, leftPipeNote2, positionListLeft.ElementAt(1), -1200);

                XYZ instancePointLeft3 = new XYZ(positionListLeft.ElementAt(2).X + lengthLeft, positionListLeft.ElementAt(2).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 700 / 304.8, 0);
                AnnotationSymbol leftPipeNote3 = CreatPipeNote(doc, positionListLeft.ElementAt(2), instancePointLeft3, typeC_Section, oneFloorPipe3_Abb + "-" + oneFloorPipe3_Size, PipeWeight(oneFloorPipe3_Size));
                MovePipeNote(doc, leftPipeNote3, positionListLeft.ElementAt(2), -1200);

                XYZ instancePointLeft4 = new XYZ(positionListLeft.ElementAt(3).X + lengthLeft, positionListLeft.ElementAt(3).Y + lengthLeft * Math.Tan(60 * Math.PI / 180) + 1050 / 304.8, 0);
                AnnotationSymbol leftPipeNote4 = CreatPipeNote(doc, positionListLeft.ElementAt(3), instancePointLeft4, typeC_Section, oneFloorPipe4_Abb + "-" + oneFloorPipe4_Size, PipeWeight(oneFloorPipe4_Size));
                MovePipeNote(doc, leftPipeNote4, positionListLeft.ElementAt(3), -1200);
            }
        }

        #endregion
        public bool HaveBrace(string nominal_Diameter) //单管是否有斜撑
        {
            bool haveBrace = false;

            if (nominal_Diameter == "200" || nominal_Diameter == "250" || nominal_Diameter == "300"
                || nominal_Diameter == "350" || nominal_Diameter == "400" || nominal_Diameter == "450")
            {
                haveBrace = true;
            }

            return haveBrace;
        }
        public bool HaveBrace(string nominal_Diameter1, string nominal_Diameter2) //双管是否有斜撑
        {
            bool haveBrace = false;

            if (nominal_Diameter1 == "125" || nominal_Diameter1 == "150" || nominal_Diameter1 == "200"
                || nominal_Diameter1 == "250" || nominal_Diameter1 == "300" || nominal_Diameter1 == "350"
                || nominal_Diameter1 == "400" || nominal_Diameter1 == "450" || nominal_Diameter2 == "125"
                || nominal_Diameter2 == "150" || nominal_Diameter2 == "200" || nominal_Diameter2 == "250"
                || nominal_Diameter2 == "300" || nominal_Diameter2 == "350" || nominal_Diameter2 == "400"
                || nominal_Diameter2 == "450")
            {
                haveBrace = true;
            }

            return haveBrace;
        }
        public bool HaveBrace(string nominal_Diameter1, string nominal_Diameter2, string nominal_Diameter3) //三管管是否有斜撑
        {
            bool haveBrace = false;

            if (nominal_Diameter1 == "125" || nominal_Diameter1 == "150" || nominal_Diameter1 == "200"
                || nominal_Diameter1 == "250" || nominal_Diameter1 == "300" || nominal_Diameter1 == "350"
                || nominal_Diameter1 == "400" || nominal_Diameter1 == "450" || nominal_Diameter2 == "125"
                || nominal_Diameter2 == "150" || nominal_Diameter2 == "200" || nominal_Diameter2 == "250"
                || nominal_Diameter2 == "300" || nominal_Diameter2 == "350" || nominal_Diameter2 == "400"
                || nominal_Diameter2 == "450" || nominal_Diameter3 == "125" || nominal_Diameter3 == "150"
                || nominal_Diameter3 == "200" || nominal_Diameter3 == "250" || nominal_Diameter3 == "300"
                || nominal_Diameter3 == "350" || nominal_Diameter3 == "400" || nominal_Diameter3 == "450"
                || nominal_Diameter1 == "100" || nominal_Diameter2 == "100" || nominal_Diameter3 == "100")
            {
                haveBrace = true;
            }

            return haveBrace;
        }

        #region 创建图名           
        public void CreatTitle(Document doc, XYZ pickpoint, FamilyInstance typeC_Section) //支架创建图名
        {
            XYZ titlePosition = new XYZ();

            if (PipeSupportSection.mainfrm.TypeA_Button.IsChecked == true)
            {
                titlePosition = new XYZ(pickpoint.X, pickpoint.Y - 700 / 304.8, 0);
            }
            if (PipeSupportSection.mainfrm.TypeB_Button.IsChecked == true)
            {
                titlePosition = new XYZ(pickpoint.X, pickpoint.Y - 700 / 304.8, 0);
            }
            if (PipeSupportSection.mainfrm.TypeC_Button.IsChecked == true)
            {
                if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true || PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
                {
                    titlePosition = new XYZ(pickpoint.X + typeC_Section.LookupParameter("一层支架长L1").AsDouble() / 2,
                     pickpoint.Y - typeC_Section.LookupParameter("支架底部膨胀螺栓定位H1").AsDouble() - 600 / 304.8, 0);
                }
                else
                {
                    titlePosition = new XYZ(pickpoint.X + typeC_Section.LookupParameter("一层长度").AsDouble() / 2, pickpoint.Y - 700 / 304.8, 0);
                }
            }
            if (PipeSupportSection.mainfrm.TypeD_Button.IsChecked == true)
            {
                titlePosition = new XYZ(pickpoint.X, pickpoint.Y - typeC_Section.LookupParameter("二层横撑定位").AsDouble() - 550 / 304.8, 0);
            }
            if (PipeSupportSection.mainfrm.TypeE_Button.IsChecked == true)
            {
                titlePosition = new XYZ(pickpoint.X, pickpoint.Y - 900 / 304.8, 0);
            }

            FamilySymbol typeC_TitleSymbol = null;
            FamilyInstance typeC_Title = null;
            typeC_TitleSymbol = TitleSymbol(doc, "图名");
            typeC_TitleSymbol.Activate();
            typeC_Title = doc.Create.NewFamilyInstance(titlePosition, typeC_TitleSymbol, doc.ActiveView);
            typeC_Title.LookupParameter("标题名称").Set(PipeSupportSection.mainfrm.SupportCode.Text);
            typeC_Title.LookupParameter("横线长度").SetValueString((PipeSupportSection.mainfrm.SupportCode.Text.Length * 5 + 10).ToString());
        }
        #endregion
        public AnnotationSymbol CreatPipeNote(Document doc, XYZ leaderPoint, XYZ instancePoint, FamilyInstance typeC_Section, string pipeAbb, string pipeWeight)//创建管道信息标注
        {
            FamilySymbol typeC_NoteSymbol = null;
            AnnotationSymbol typeC_Note = null;
            typeC_NoteSymbol = TitleSymbol(doc, "支架剖面管道标注");
            typeC_NoteSymbol.Activate();
            typeC_Note = doc.Create.NewFamilyInstance(instancePoint, typeC_NoteSymbol, doc.ActiveView) as AnnotationSymbol;
            typeC_Note.LookupParameter("管道类型及尺寸").Set(pipeAbb);
            typeC_Note.LookupParameter("管道重量").Set(pipeWeight);
            typeC_Note.addLeader();
            IList<Leader> leadList = typeC_Note.GetLeaders();
            Leader lead = leadList[0];
            lead.End = leaderPoint;

            return typeC_Note;
        }

        #region 创建支架尺寸标注
        public void TypeA_CreatDimensionX(Document doc, FamilyInstance section, XYZ pickPoint) //A型支架创建X方向尺寸标注
        {
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList = new List<Line>();
            List<Line> lineList1 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            double height = 500;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层右侧管道中心线");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + 2200 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧内壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧内壁");//将实例中弱参照参照平面拿出来
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层右侧管道中心线");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("一层高度H").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 450 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧内壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧内壁");//将实例中弱参照参照平面拿出来
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层右侧管道中心线");
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑左侧边界", "二层横撑右侧边界");
                if (lineList.Count != 0 && lineList1.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    if (!cableTray_Check)
                    {
                        height = section.LookupParameter("二层高度H").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 500 / 304.8, 0);
                    }
                    else
                    {
                        height = section.LookupParameter("三层高度H").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                    }
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeB_CreatDimensionX(Document doc, FamilyInstance section, XYZ pickPoint) //B型支架创建X方向尺寸标注
        {
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList = new List<Line>();
            List<Line> lineList1 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            double height = 500;
            double dimensionHeight = 450;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (!cableTray_Check)
                {
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层左侧管道中心线", "三层右侧管道中心线");
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑左侧边界", "三层横撑右侧边界");
                    if (lineList.Count != 0 && lineList1.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("三层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 500 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
                else
                {
                    //一层左侧管道尺寸标注
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层横撑左侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("二层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("左侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                    //一层右侧管道尺寸标注
                    referenceArray.Clear();
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层右侧管道中心线", "二层横撑右侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("二层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("右侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                if (!cableTray_Check)
                {
                    //一层左侧管道尺寸标注
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层横撑左侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("二层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("左侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                    //一层右侧管道尺寸标注
                    referenceArray.Clear();
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层右侧管道中心线", "二层横撑右侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("二层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("右侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
                else
                {
                    //一层左侧管道尺寸标注
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层横撑左侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("一层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("左侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                    //一层右侧管道尺寸标注
                    referenceArray.Clear();
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层右侧管道中心线", "一层横撑右侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("一层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("右侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                if (!cableTray_Check)
                {
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层左侧管道中心线", "三层右侧管道中心线");
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑左侧边界", "三层横撑右侧边界");
                    if (lineList.Count != 0 && lineList1.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("三层支架高度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 500 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
                else
                {
                    //二层左侧管道尺寸标注
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层横撑左侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("二层支架高度").AsDouble();
                        dimensionHeight = section.LookupParameter("二层与三层间距H").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + dimensionHeight + 50 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("左侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                    //二层右侧管道尺寸标注
                    referenceArray.Clear();
                    lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层右侧管道中心线", "二层横撑右侧边界");
                    if (lineList.Count != 0)
                    {
                        tempLine = lineList.FirstOrDefault();
                        tempLine.MakeUnbound();

                        height = section.LookupParameter("二层支架高度").AsDouble();
                        dimensionHeight = section.LookupParameter("二层与三层间距H").AsDouble();
                        origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + dimensionHeight + 50 / 304.8, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref2 = section.GetReferenceByName("右侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref2 != null)
                        {
                            referenceArray.Append(ref2);
                        }
                        foreach (Line item in lineList)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
            }

            if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                //一层左侧管道尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层横撑左侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("一层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref2 = section.GetReferenceByName("左侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref2 != null)
                    {
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //一层右侧管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层右侧管道中心线", "一层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("一层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref2 = section.GetReferenceByName("右侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref2 != null)
                    {
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                //二层左侧管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层横撑左侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("三层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 230 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref2 = section.GetReferenceByName("左侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref2 != null)
                    {
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层右侧管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层右侧管道中心线", "二层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("三层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 230 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref2 = section.GetReferenceByName("右侧支柱外壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref2 != null)
                    {
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //三层管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层左侧管道中心线", "三层右侧管道中心线");
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑左侧边界", "三层横撑右侧边界");
                if (lineList.Count != 0 && lineList1.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("三层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 500 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeC_CreatDimensionX(Document doc, FamilyInstance section, string supportBoundary, string pipeCenterLine, XYZ pickPoint) //C型支架创建X方向尺寸标注
        {
            List<Line> lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, supportBoundary, pipeCenterLine);
            if (section.LookupParameter("二层支架竖撑显隐").AsInteger() == 1)
            {
                //lineList.RemoveAt(0);
            }
            //MessageBox.Show(lineList.Count.ToString());

            ReferenceArray refArray = new ReferenceArray();
            foreach (Line item in lineList)
            {
                refArray.Append(item.Reference);
            }
            Line tempLine = lineList.FirstOrDefault();
            tempLine.MakeUnbound();
            pickPoint = new XYZ(pickPoint.X, pickPoint.Y, 0);
            XYZ targetPoint = tempLine.Project(pickPoint).XYZPoint;
            XYZ direction = (targetPoint - pickPoint).Normalize();
            Line dimLine = Line.CreateUnbound(pickPoint, direction);

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            doc.Create.NewDimension(doc.ActiveView, dimLine, refArray, dimType);
        }
        public void TypeC_CreatDimensionX(Document doc, FamilyInstance section, XYZ pickPoint) //C型支架创建X方向尺寸标注用于三四层管道
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            double height = 500;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                //一层支架尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层管道中心线", "一层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照          
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层支架尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层管道中心线", "二层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("三层高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 250 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照              
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //三层支架尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层管道中心线", "三层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("三层高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 450 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照             
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            if (PipeSupportSection.mainfrm.FourFloor.IsChecked == true)
            {
                //一层支架尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层管道中心线", "一层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照                
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层支架尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层管道中心线", "二层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("三层高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 250 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照             
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //三层支架尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层管道中心线", "三层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("四层高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 250 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照              
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //四层支架尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "四层管道中心线", "四层横撑右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("四层高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 450 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("左侧支架边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照           
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeD_CreatDimensionX(Document doc, FamilyInstance section, XYZ pickPoint) //D型支架创建X方向尺寸标注
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            double height = 500;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层右侧管道中心线");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("一层高度H").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - height - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧内壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧内壁");//将实例中弱参照参照平面拿出来
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层右侧管道中心线");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("一层高度H").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - height + 450 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧内壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧内壁");//将实例中弱参照参照平面拿出来
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层右侧管道中心线");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("二层横撑定位").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - height - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧内壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧内壁");//将实例中弱参照参照平面拿出来
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeE_CreatDimensionX(Document doc, FamilyInstance section, XYZ pickPoint) //E型支架创建X方向尺寸标注
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList = new List<Line>();
            List<Line> lineList1 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            double height = 500;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层右侧管道中心线");
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑左侧边界", "一层横撑右侧边界");
                if (lineList.Count != 0&& lineList1.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + 600 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //支墩长度尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "支墩左侧边界", "支墩右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - 700 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }                  
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层左侧管道中心线", "一层右侧管道中心线");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("二层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height - 250 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧内壁");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧内壁");//将实例中弱参照参照平面拿出来
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层左侧管道中心线", "二层右侧管道中心线");
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑左侧边界", "二层横撑右侧边界");
                if (lineList.Count != 0&& lineList1.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    height = section.LookupParameter("二层支架高度").AsDouble();
                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y + height + 500 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);
                  
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //支墩长度尺寸标注
                referenceArray.Clear();
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "支墩左侧边界", "支墩右侧边界");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - 700 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeF_CreatDimensionX(Document doc, FamilyInstance section, XYZ pickPoint) //F型支架创建X方向尺寸标注
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            double height = 500;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层管道中心线", "");
                if (lineList.Count != 0)
                {
                    tempLine = lineList.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X, pickPoint.Y - 300 / 304.8, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("支架左侧边界");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    ref2 = section.GetReferenceByName("支架右侧边界");//将实例中弱参照参照平面拿出来                 
                    if (ref1 != null && ref2 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        referenceArray.Append(ref2);
                    }
                    foreach (Line item in lineList)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeA_CreatDimensionY(Document doc, FamilyInstance section, XYZ pickPoint) //A型支架创建Y方向尺寸标注
        {
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList1 = new List<Line>();
            List<Line> lineList2 = new List<Line>();
            List<Line> lineList3 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            XYZ textPosition = new XYZ();
            double width = 500;
            Dimension dimension = null;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (!cableTray_Check)
                {
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部轮廓线");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "6000", textPosition);
                    }
                }
                else
                {
                    //一层管道尺寸标注
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部轮廓线");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//特别注意中心前后及中心左右的参照平面不能通过名称获取
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "6000", textPosition);
                    }

                    //二层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部轮廓线");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑底部轮廓线");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }

            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                if (!cableTray_Check)
                {
                    //一层管道尺寸标注
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部轮廓线");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//特别注意中心前后及中心左右的参照平面不能通过名称获取
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "6000", textPosition);
                    }

                    //二层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部轮廓线");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部轮廓线");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                }
                else
                {
                    //一层管道尺寸标注
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部轮廓线");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//特别注意中心前后及中心左右的参照平面不能通过名称获取
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "6000", textPosition);
                    }

                    //二层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部轮廓线");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部轮廓线");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                    //三层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部轮廓线");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑底部轮廓线");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("支架长度").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
            }

            referenceArray.Clear();
        }
        public void TypeB_CreatDimensionY(Document doc, FamilyInstance section, XYZ pickPoint) //B型支架创建Y方向尺寸标注
        {
            bool cableTray_Check = (bool)PipeSupportSection.mainfrm.CableTray.IsChecked;
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList1 = new List<Line>();
            List<Line> lineList2 = new List<Line>();
            List<Line> lineList3 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            XYZ textPosition = new XYZ();
            double width = 500;
            List<double> widthList = new List<double>();
            Dimension dimension = null;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                if (!cableTray_Check)
                {
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑顶部边界");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("三层横撑长度L").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width / 2 - 500 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "3500", textPosition);
                    }
                }
                else
                {
                    //一层管道尺寸标注
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("二层左侧横撑长度L").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//特别注意中心前后及中心左右的参照平面不能通过名称获取
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "3500", textPosition);
                    }

                    //二层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "电缆桥架底部边界");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = section.LookupParameter("二层左侧横撑长度L").AsDouble();
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }

            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                if (!cableTray_Check)
                {
                    //一层管道尺寸标注
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        if ((section.LookupParameter("二层左侧横撑长度L").AsDouble() + 100 / 304.8) > section.LookupParameter("三层横撑长度L").AsDouble() / 2)
                        {
                            width = section.LookupParameter("二层左侧横撑长度L").AsDouble();
                        }
                        else
                        {
                            width = section.LookupParameter("三层横撑长度L").AsDouble() / 2;
                        }
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "3500", textPosition);
                    }

                    //二层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑底部边界");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        if ((section.LookupParameter("二层左侧横撑长度L").AsDouble() + 100 / 304.8) > section.LookupParameter("三层横撑长度L").AsDouble() / 2)
                        {
                            width = section.LookupParameter("二层左侧横撑长度L").AsDouble();
                        }
                        else
                        {
                            width = section.LookupParameter("三层横撑长度L").AsDouble() / 2;
                        }
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
                else
                {
                    widthList.Add(section.LookupParameter("一层左侧横撑长度L").AsDouble() + 100 / 304.8);
                    widthList.Add(section.LookupParameter("二层左侧横撑长度L").AsDouble() + 100 / 304.8);
                    widthList.Add(section.LookupParameter("三层横撑长度L").AsDouble() / 2);
                    widthList.Sort();
                    //一层管道尺寸标注
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                    if (lineList1.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = widthList[widthList.Count - 1];
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//特别注意中心前后及中心左右的参照平面不能通过名称获取
                        if (ref1 != null)
                        {
                            referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                        }
                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                        dimension.ValueOverride = "\u200E";
                        textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                        VerticalDimensionText(doc, doc.ActiveView, "3500", textPosition);
                    }

                    //二层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部边界");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = widthList[widthList.Count - 1];
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }

                    //三层管道尺寸标注
                    referenceArray.Clear();
                    lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                    lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "电缆桥架底部边界");
                    if (lineList1.Count != 0 && lineList2.Count != 0)
                    {
                        tempLine = lineList1.FirstOrDefault();
                        tempLine.MakeUnbound();

                        width = widthList[widthList.Count - 1];
                        origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                        targetPoint = tempLine.Project(origionPoint).XYZPoint;
                        direction = (targetPoint - origionPoint).Normalize();
                        dimLine = Line.CreateUnbound(origionPoint, direction);

                        foreach (Line item in lineList1)
                        {
                            referenceArray.Append(item.Reference);
                        }
                        foreach (Line item in lineList2)
                        {
                            referenceArray.Append(item.Reference);
                        }

                        doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    }
                }
            }

            if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                widthList.Add(section.LookupParameter("一层左侧横撑长度L").AsDouble() + 100 / 304.8);
                widthList.Add(section.LookupParameter("二层左侧横撑长度L").AsDouble() + 100 / 304.8);
                widthList.Add(section.LookupParameter("三层横撑长度L").AsDouble() / 2);
                widthList.Sort();

                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                if (lineList1.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = widthList[widthList.Count - 1];
                    origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//特别注意中心前后及中心左右的参照平面不能通过名称获取
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    dimension = doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                    dimension.ValueOverride = "\u200E";
                    textPosition = new XYZ(dimension.TextPosition.X, dimension.TextPosition.Y + doc.ActiveView.Scale * 3.5 / 304.8, 0);
                    VerticalDimensionText(doc, doc.ActiveView, "3500", textPosition);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = widthList[widthList.Count - 1];
                    origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //三层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = widthList[widthList.Count - 1];
                    origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeC_CreatDimensionY(Document doc, FamilyInstance section, string supportBoundary, XYZ pickPoint) //创建Y方向尺寸标注
        {
            List<Line> lineList = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, supportBoundary);
            ReferenceArray refArray = new ReferenceArray();
            foreach (Line item in lineList)
            {
                refArray.Append(item.Reference);
            }
            Line tempLine = lineList.FirstOrDefault();
            tempLine.MakeUnbound();
            pickPoint = new XYZ(pickPoint.X, pickPoint.Y, 0);
            XYZ targetPoint = tempLine.Project(pickPoint).XYZPoint;
            XYZ direction = (targetPoint - pickPoint).Normalize();
            Line dimLine = Line.CreateUnbound(pickPoint, direction);

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            doc.Create.NewDimension(doc.ActiveView, dimLine, refArray, dimType);
        }
        public void TypeC_CreatDimensionY(Document doc, FamilyInstance section, XYZ pickPoint) //C型支架创建Y方向尺寸标注用于三四层管道
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList1 = new List<Line>();
            List<Line> lineList2 = new List<Line>();
            List<Line> lineList3 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            XYZ textPosition = new XYZ();
            double width = 500;
            List<double> widthList = new List<double>();
            Dimension dimension = null;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.ThreeFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X - 350 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X - 350 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }
           
            if (PipeSupportSection.mainfrm.FourFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X - 350 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X - 350 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //三层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "三层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "四层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    origionPoint = new XYZ(pickPoint.X - 350 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        public void TypeD_CreatDimensionY(Document doc, FamilyInstance section, XYZ pickPoint) //D型支架创建Y方向尺寸标注
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList1 = new List<Line>();
            List<Line> lineList2 = new List<Line>();
            List<Line> lineList3 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            XYZ textPosition = new XYZ();
            double width = 500;
            List<double> widthList = new List<double>();
            Dimension dimension = null;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                if (lineList1.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = section.LookupParameter("支架长度L").AsDouble()/2;
                    origionPoint = new XYZ(pickPoint.X -width-400 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }                 
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }             
            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                if (lineList1.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = section.LookupParameter("支架长度L").AsDouble() / 2;
                    origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = section.LookupParameter("支架长度L").AsDouble() / 2;
                    origionPoint = new XYZ(pickPoint.X - width - 400 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }           
            }

            referenceArray.Clear();
        }
        public void TypeE_CreatDimensionY(Document doc, FamilyInstance section, XYZ pickPoint) //E型支架创建Y方向尺寸标注
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Reference ref1 = null;
            Reference ref2 = null;
            List<Line> lineList1 = new List<Line>();
            List<Line> lineList2 = new List<Line>();
            List<Line> lineList3 = new List<Line>();
            Line tempLine = null;
            Line dimLine = null;
            XYZ targetPoint = new XYZ();
            XYZ direction = new XYZ();
            XYZ origionPoint = new XYZ();
            XYZ textPosition = new XYZ();
            double width = 500;
            List<double> widthList = new List<double>();
            Dimension dimension = null;

            DimensionType dimType = null;
            FilteredElementCollector dimTypeCollector = new FilteredElementCollector(doc);
            dimTypeCollector.OfClass(typeof(DimensionType));
            IList<Element> dimTypes = dimTypeCollector.ToElements();
            foreach (DimensionType item in dimTypes)
            {
                if (item.Name.Contains("工艺")) //给排水标注样式调用有问题
                {
                    dimType = item;
                    break;
                }
            }

            if (PipeSupportSection.mainfrm.OneFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "支墩顶部边界");
                if (lineList1.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = section.LookupParameter("支架长度L").AsDouble() / 2;
                    origionPoint = new XYZ(pickPoint.X - width - 300 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("室外地坪");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            if (PipeSupportSection.mainfrm.TwoFloor.IsChecked == true)
            {
                //一层管道尺寸标注
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "支墩顶部边界");
                if (lineList1.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = section.LookupParameter("支架长度L").AsDouble() / 2;
                    origionPoint = new XYZ(pickPoint.X - width - 300 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    ref1 = section.GetReferenceByName("室外地坪");//将实例中通过参照平面名称获取参照，前提是参照平面必须不能为非参照
                    if (ref1 != null)
                    {
                        referenceArray.Append(ref1);//将获得的中线放入参照平台面内
                    }
                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }

                //二层管道尺寸标注
                referenceArray.Clear();
                lineList1 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "一层横撑顶部边界");
                lineList2 = GetReferenceOfDetailComponent(doc, section, doc.ActiveView, "二层横撑底部边界");
                if (lineList1.Count != 0 && lineList2.Count != 0)
                {
                    tempLine = lineList1.FirstOrDefault();
                    tempLine.MakeUnbound();

                    width = section.LookupParameter("支架长度L").AsDouble() / 2;
                    origionPoint = new XYZ(pickPoint.X - width - 300 / 304.8, pickPoint.Y, 0);
                    targetPoint = tempLine.Project(origionPoint).XYZPoint;
                    direction = (targetPoint - origionPoint).Normalize();
                    dimLine = Line.CreateUnbound(origionPoint, direction);

                    foreach (Line item in lineList1)
                    {
                        referenceArray.Append(item.Reference);
                    }
                    foreach (Line item in lineList2)
                    {
                        referenceArray.Append(item.Reference);
                    }

                    doc.Create.NewDimension(doc.ActiveView, dimLine, referenceArray, dimType);
                }
            }

            referenceArray.Clear();
        }
        #endregion
        public void MovePipeNote(Document doc, AnnotationSymbol pipeNote, XYZ centerPoint, double moveLengh)
        {
            ElementTransformUtils.MoveElement(doc, pipeNote.Id, new XYZ(moveLengh / 304.8, 0, 0));
            IList<Leader> leadList = pipeNote.GetLeaders();
            Leader lead = leadList[0];
            lead.End = centerPoint;
        }
        public TextNote VerticalDimensionText(Document doc, View view, string text, XYZ point)
        {
            TextNote textValue = null;
            TextNoteType type = null;
            IList<TextNoteType> noteTypes = CollectorHelper.TCollector<TextNoteType>(doc);

            foreach (var item in noteTypes)
            {
                if (item.Name.Contains("给排水-字高3.5"))
                {
                    type = item;
                    break;
                }
            }

            textValue = TextNote.Create(doc, view.Id, point, text, type.Id);
            Line zAxis = Line.CreateBound(new XYZ(point.X, point.Y, 0), new XYZ(point.X, point.Y, 1));
            ElementTransformUtils.RotateElement(doc, textValue.Id, zAxis, -Math.PI / 2);
            return textValue;
        }
        private static List<Line> GetReferenceOfDetailComponent(Document doc
            , Element element, View view, string supportBoundary, string pipeCenterLine) //获取详图项目尺寸标注参照
        {
            List<Line> lineList = new List<Line>();
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Line line = geomObjTmp as Line;
                        if (line != null)
                        {
                            if (line.Direction.Y == -1 || line.Direction.Y == 1)
                            {
                                ElementId styleID = line.GraphicsStyleId;
                                GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                                if (style.Name == supportBoundary || style.Name == pipeCenterLine)
                                {
                                    lineList.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            return lineList;
        }
        private static List<Line> GetReferenceOfDetailComponent(Document doc
            , Element element, View view, string supportBoundary) //获取详图项目尺寸标注参照
        {
            List<Line> lineList = new List<Line>();
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Line line = geomObjTmp as Line;
                        if (line != null)
                        {
                            if (line.Direction.X == -1 || line.Direction.X == 1)
                            {
                                ElementId styleID = line.GraphicsStyleId;
                                GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                                if (style.Name == supportBoundary)
                                {
                                    lineList.Add(line);
                                }
                            }
                        }
                    }
                }
            }
            return lineList;
        }
        public double MaximumDiameter(Document doc, Element element, View view, string pipeOutline) //获取每层直径最大的管道
        {
            double max = 0;
            List<double> arcList = new List<double>();

            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Arc arc = geomObjTmp as Arc;
                        if (arc != null)
                        {
                            ElementId styleID = arc.GraphicsStyleId;
                            GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                            if (style.Name.Contains(pipeOutline))
                            {
                                arcList.Add(arc.Radius);
                            }
                        }
                    }
                }
            }
            arcList.Sort();

            return max = arcList.ElementAt(arcList.Count - 1) * 2;
        }
        public List<XYZ> PipeCenterPosition(Document doc, FamilyInstance element, View view, string pipeOutline)//获取管道中心定位
        {

            List<XYZ> arcList = new List<XYZ>();

            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = false;
            if (view != null)
            {
                options.View = view;
            }
            else
            {
                options.DetailLevel = ViewDetailLevel.Fine;
            }

            var geoElem = element.get_Geometry(options);
            foreach (var item in geoElem)
            {
                GeometryInstance geoInst = item as GeometryInstance;
                if (geoInst != null)
                {
                    GeometryElement geoElemTmp = geoInst.GetSymbolGeometry();
                    foreach (GeometryObject geomObjTmp in geoElemTmp)
                    {
                        Arc arc = geomObjTmp as Arc;
                        if (arc != null)
                        {
                            ElementId styleID = arc.GraphicsStyleId;
                            GraphicsStyle style = doc.GetElement(styleID) as GraphicsStyle;
                            if (style.Name.Contains(pipeOutline))
                            {
                                Transform trans = element.GetTransform();
                                arcList.Add(trans.OfPoint(arc.Center));
                            }
                        }
                    }
                }
            }
            //arcList.Sort();
            return arcList;
        }
        public Tuple<int, int, int> PipeDistance(string nominal_Diameter) //管道间距
        {
            int distance1 = 0; //  距墙间距L1
            int distance2 = 0; //  管道之间间距L2
            int distance3 = 0; //  距支架边缘间距L3
            if (PipeSupportSection.mainfrm.Insulation.IsChecked == true)
            {
                if (nominal_Diameter == "15")
                {
                    distance1 = 130;
                    distance2 = 190;
                    distance3 = 100;
                }
                else if (nominal_Diameter == "20")
                {
                    distance1 = 140;
                    distance2 = 190;
                    distance3 = 100;
                }
                else if (nominal_Diameter == "25")
                {
                    distance1 = 140;
                    distance2 = 200;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "32")
                {
                    distance1 = 150;
                    distance2 = 210;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "40")
                {
                    distance1 = 160;
                    distance2 = 210;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "50")
                {
                    distance1 = 160;
                    distance2 = 230;
                    distance3 = 120;
                }
                else if (nominal_Diameter == "65")
                {
                    distance1 = 170;
                    distance2 = 250;
                    distance3 = 130;
                }
                else if (nominal_Diameter == "80")
                {
                    distance1 = 190;
                    distance2 = 260;
                    distance3 = 140;
                }
                else if (nominal_Diameter == "100")
                {
                    distance1 = 200;
                    distance2 = 300;
                    distance3 = 150;
                }
                else if (nominal_Diameter == "125")
                {
                    distance1 = 220;
                    distance2 = 320;
                    distance3 = 170;
                }
                else if (nominal_Diameter == "150")
                {
                    distance1 = 230;
                    distance2 = 350;
                    distance3 = 180;
                }
                else if (nominal_Diameter == "200")
                {
                    distance1 = 260;
                    distance2 = 400;
                    distance3 = 210;
                }
                else if (nominal_Diameter == "250")
                {
                    distance1 = 290;
                    distance2 = 470;
                    distance3 = 250;
                }
                else if (nominal_Diameter == "300")
                {
                    distance1 = 330;
                    distance2 = 520;
                    distance3 = 270;
                }
                else if (nominal_Diameter == "350")
                {
                    distance1 = 360;
                    distance2 = 580;
                    distance3 = 300;
                }
                else if (nominal_Diameter == "400")
                {
                    distance1 = 390;
                    distance2 = 640;
                    distance3 = 330;
                }
                else if (nominal_Diameter == "450")
                {
                    distance1 = 420;
                    distance2 = 700;
                    distance3 = 360;
                }
            }
            else
            {
                if (nominal_Diameter == "15")
                {
                    distance1 = 70;
                    distance2 = 100;
                    distance3 = 40;
                }
                else if (nominal_Diameter == "20")
                {
                    distance1 = 80;
                    distance2 = 110;
                    distance3 = 40;
                }
                else if (nominal_Diameter == "25")
                {
                    distance1 = 80;
                    distance2 = 120;
                    distance3 = 50;
                }
                else if (nominal_Diameter == "32")
                {
                    distance1 = 90;
                    distance2 = 140;
                    distance3 = 50;
                }
                else if (nominal_Diameter == "40")
                {
                    distance1 = 100;
                    distance2 = 150;
                    distance3 = 50;
                }
                else if (nominal_Diameter == "50")
                {
                    distance1 = 100;
                    distance2 = 170;
                    distance3 = 60;
                }
                else if (nominal_Diameter == "65")
                {
                    distance1 = 110;
                    distance2 = 190;
                    distance3 = 70;
                }
                else if (nominal_Diameter == "80")
                {
                    distance1 = 130;
                    distance2 = 210;
                    distance3 = 80;
                }
                else if (nominal_Diameter == "100")
                {
                    distance1 = 140;
                    distance2 = 240;
                    distance3 = 90;
                }
                else if (nominal_Diameter == "125")
                {
                    distance1 = 160;
                    distance2 = 260;
                    distance3 = 110;
                }
                else if (nominal_Diameter == "150")
                {
                    distance1 = 170;
                    distance2 = 300;
                    distance3 = 120;
                }
                else if (nominal_Diameter == "200")
                {
                    distance1 = 200;
                    distance2 = 350;
                    distance3 = 150;
                }
                else if (nominal_Diameter == "250")
                {
                    distance1 = 230;
                    distance2 = 410;
                    distance3 = 190;
                }
                else if (nominal_Diameter == "300")
                {
                    distance1 = 270;
                    distance2 = 460;
                    distance3 = 210;
                }
                else if (nominal_Diameter == "350")
                {
                    distance1 = 300;
                    distance2 = 530;
                    distance3 = 240;
                }
                else if (nominal_Diameter == "400")
                {
                    distance1 = 330;
                    distance2 = 590;
                    distance3 = 270;
                }
                else if (nominal_Diameter == "450")
                {
                    distance1 = 360;
                    distance2 = 650;
                    distance3 = 300;
                }
            }

            Tuple<int, int, int> tup = new Tuple<int, int, int>(distance1, distance2, distance3);
            return tup;
        }
        public int GetFloorHeight(int pipeSize)
        {
            int height = 500;
            if (pipeSize == 15 || pipeSize == 20 || pipeSize == 25 || pipeSize == 32 || pipeSize == 40 || pipeSize == 50)
            {
                height = 200;
            }
            else if (pipeSize == 65 || pipeSize == 80 || pipeSize == 100 || pipeSize == 125 || pipeSize == 150)
            {
                height = 300;
            }
            else if (pipeSize == 200 || pipeSize == 250)
            {
                height = 400;
            }
            else if (pipeSize == 300 || pipeSize == 350)
            {
                height = 500;
            }
            else if (pipeSize == 400 || pipeSize == 450)
            {
                height = 600;
            }

            return height;
        }
        public int GetPipeDistance10(double num)
        {
            return (int)Math.Ceiling(num / 10) * 10;
        }
        public void DetailDrawingFamilyLoad(Document doc, string categoryName)
        {
            IList<Element> familyCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family family = null;
            foreach (Family item in familyCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    family = item;
                    break;
                }
            }
            if (family == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_详图项目_" + categoryName + ".rfa");
            }
        }
        public FamilySymbol PipeSupportSectionSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DetailComponents);

            IList<Element> sections = sectionCollector.ToElements();
            foreach (FamilySymbol item in sections)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(symbolName))
                {
                    symbol = item;
                    break;
                }
            }
            return symbol;
        }
        public void DetailDrawingTitleLoad(Document doc, string categoryName)
        {
            IList<Element> titleCollect = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToElements();
            Family title = null;

            foreach (Family item in titleCollect)
            {
                if (item.Name.Contains(categoryName) && item.Name.Contains("给排水"))
                {
                    title = item;
                    break;
                }
            }
            if (title == null)
            {
                doc.LoadFamily(@"C:\ProgramData\Autodesk\Revit\Addins\2018\FFETOOLS\Family\" + "给排水_注释符号_" + categoryName + ".rfa");
            }
        }
        public FamilySymbol TitleSymbol(Document doc, string symbolName)
        {
            FamilySymbol symbol = null;
            FilteredElementCollector sectionCollector = new FilteredElementCollector(doc);
            sectionCollector.OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericAnnotation);

            IList<Element> sections = sectionCollector.ToElements();
            foreach (FamilySymbol item in sections)
            {
                if (item.Family.Name.Contains("给排水") && item.Family.Name.Contains(symbolName))
                {
                    symbol = item;
                    break;
                }
            }
            return symbol;
        }
        public string PipeWeight(string nominal_Diameter) //管道重量
        {
            string weight = null;
            if (PipeSupportSection.mainfrm.Insulation.IsChecked == true)
            {
                if (nominal_Diameter == "DN15")
                {
                    weight = "3.3";
                }
                else if (nominal_Diameter == "DN20")
                {
                    weight = "4.0";
                }
                else if (nominal_Diameter == "DN25")
                {
                    weight = "5.3";
                }
                else if (nominal_Diameter == "DN32")
                {
                    weight = "6.7";
                }
                else if (nominal_Diameter == "DN40")
                {
                    weight = "7.9";
                }
                else if (nominal_Diameter == "DN50")
                {
                    weight = "10.3";
                }
                else if (nominal_Diameter == "DN65")
                {
                    weight = "14.2";
                }
                else if (nominal_Diameter == "DN80")
                {
                    weight = "17.8";
                }
                else if (nominal_Diameter == "DN100")
                {
                    weight = "25.3";
                }
                else if (nominal_Diameter == "DN125")
                {
                    weight = "34.0";
                }
                else if (nominal_Diameter == "DN150")
                {
                    weight = "45.3";
                }
                else if (nominal_Diameter == "DN200")
                {
                    weight = "77.6";
                }
                else if (nominal_Diameter == "DN250")
                {
                    weight = "112.4";
                }
                else if (nominal_Diameter == "DN300")
                {
                    weight = "155.7";
                }
                else if (nominal_Diameter == "DN350")
                {
                    weight = "211.0";
                }
                else if (nominal_Diameter == "DN400")
                {
                    weight = "255.8";
                }
                else if (nominal_Diameter == "DN450")
                {
                    weight = "295.7";
                }
            }
            else
            {
                if (nominal_Diameter == "DN15")
                {
                    weight = "1.7";
                }
                else if (nominal_Diameter == "DN20")
                {
                    weight = "2.2";
                }
                else if (nominal_Diameter == "DN25")
                {
                    weight = "3.3";
                }
                else if (nominal_Diameter == "DN32")
                {
                    weight = "4.6";
                }
                else if (nominal_Diameter == "DN40")
                {
                    weight = "5.7";
                }
                else if (nominal_Diameter == "DN50")
                {
                    weight = "7.8";
                }
                else if (nominal_Diameter == "DN65")
                {
                    weight = "11.3";
                }
                else if (nominal_Diameter == "DN80")
                {
                    weight = "14.8";
                }
                else if (nominal_Diameter == "DN100")
                {
                    weight = "21.7";
                }
                else if (nominal_Diameter == "DN125")
                {
                    weight = "29.9";
                }
                else if (nominal_Diameter == "DN150")
                {
                    weight = "40.7";
                }
                else if (nominal_Diameter == "DN200")
                {
                    weight = "71.8";
                }
                else if (nominal_Diameter == "DN250")
                {
                    weight = "105.5";
                }
                else if (nominal_Diameter == "DN300")
                {
                    weight = "147.7";
                }
                else if (nominal_Diameter == "DN350")
                {
                    weight = "201.9";
                }
                else if (nominal_Diameter == "DN400")
                {
                    weight = "245.7";
                }
                else if (nominal_Diameter == "DN450")
                {
                    weight = "284.8";
                }
            }
            return weight;
        }
    }
}
