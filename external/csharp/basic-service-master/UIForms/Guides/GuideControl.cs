using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceMaster.UIForms.Guides
{
    public class GuideControl
    {
        private List<GuideBase> guideForms;
        private int currentFormIndex;
        private List<ServiceChange> changeList;

        public List<ServiceChange> ChangeList
        {
            get { return changeList; }
            set { changeList = value; }
        }
        public int CurrentFormIndex
        {
            get { return currentFormIndex; }
            set { currentFormIndex = value; }
        }
        public List<GuideBase> GuideForms
        {
            get { return guideForms; }
            set { guideForms = value; }
        }


        public GuideControl(GuideBase[] forms)
        {
            guideForms = new List<GuideBase>();
            ChangeList = new List<ServiceChange>();
            foreach (GuideBase form in forms)
            {
                form.Controller = this;
                guideForms.Add(form);
            }
            guideForms[guideForms.Count - 1].setFinish();
        }

        public void doNext()
        {
            if (currentFormIndex < guideForms.Count-2)
            {
                guideForms[currentFormIndex].Visible = false;
                currentFormIndex++;
                if (guideForms[currentFormIndex].detect())
                    guideForms[currentFormIndex].Show();
                else
                    doNext();
            }
            else if(currentFormIndex == guideForms.Count-2)
            {
                guideForms[currentFormIndex].Visible = false;
                currentFormIndex++;
                (guideForms[currentFormIndex] as GuideFinish).setData();
                if (guideForms[currentFormIndex].detect())
                    guideForms[currentFormIndex].Show();
                else
                    doNext();
            }
            else if (currentFormIndex == guideForms.Count - 1)
            {
                foreach (ServiceChange sc in changeList)
                    sc.doChange();
                guideForms[currentFormIndex].Visible = false;
                closeAll();
            }
            return;
        }
        public void closeAll()
        {
            foreach (GuideBase guide in guideForms)
                guide.Dispose();
        }
        public void doStart()
        {
            guideForms[0].Show();
        }
    }
}
