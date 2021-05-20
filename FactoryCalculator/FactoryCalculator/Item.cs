using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
namespace FactoryCalculator
{
    [DataContract]
    public class Item :ICloneable
    {
        [DataMember]

        public List<Detail> Details;

        [DataMember]

        public string Name { get; set; }

        [DataMember]

        public int ID { get; set; }

        public int Step { get; set; }

        protected Item()
        {

        }
        public Item(string name, int id, List<Detail> details = null)
        {
            Name = name;
            ID = id;
            Step = 0;
            Details = details;

        }


        public List<Detail> GetStepsChain(Item item)
        {
            var tempList = new List<Detail>();
            var json = new DataContractJsonSerializer(typeof(List<Item>));

            List<Item> tempListItems = new List<Item>();

            Item tempItem = default;
             for (int i = item.Step; i>0;i--)
             {

                 tempListItems = Program.ReadObjects();

                tempItem = tempListItems.Where(x => x.Name == item.Name).FirstOrDefault();
                tempItem.GetStep();


                 tempList.AddRange(GetStep(new Detail(tempItem, 1), i));





             }


           
            return tempList;



        }


        public List<Detail> GetStep(Detail preview, int step, float multiplier=1f )
        {

            float tempMultiplier = multiplier * preview.count;
            var tempList = new List<Detail>();
            Detail tempDet;

            if (Step == step)
            {
                tempDet = (Detail)preview.Clone();
                tempDet.count *= multiplier;
                tempList.Add(tempDet);
            }
            else
            {
                foreach (var det in Details)
                {

                    

                    

                    tempList.AddRange(det.item.GetStep(det, step, tempMultiplier));
                }
            }

            return tempList;
        }


        public void GetStep(int previewStep =0)
        {

            if (Details == null || Details.Count==0)
            {
                this.Step = previewStep+1;
            }
            else
            {
               
                foreach(var det in Details)
                {
                   det.item.GetStep(Step);


                }


                Step = Details.Max(x => x.item.Step) + 1;

            }


        }

        public object Clone()
        {

            List<Detail> list = new List<Detail>();

            foreach(var det in Details)
            {
                list.Add((Detail)det.Clone());
            }
            return new Item(this.Name, this.ID,list )
            {
                Step = this.Step
            
            
            };
        }
    }
}
