using Arch.Data;
using Arch.Data.Common.constant;
using Arch.Data.Orm;
using DAL.DbEngine.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {

            BaseDao dao = BaseDaoFactory.CreateBaseDao("dao_test");

            //添加
            for (int i = 0; i < 100000; i++)
            {
                dao.Insert<test0>(new test0()
                {
                    Address = "上海" + i,
                    Id = i,
                    Name = "王" + i
                });
                Console.WriteLine(i);
            }

            //分页查询
            //IList<string> ilist = new List<string> { "0", "1" };
            //IDictionary hints = new Dictionary<string, object>();
            //hints.Add(DALExtStatementConstant.SHARD_IDS, ilist);
            //var query = dao.GetQuery<test0>().Paging(1, 10, "Id", false);//.Equal("Name", "王10000");


            //while (true)
            //{
            //    var list_ = dao.SelectList<test0>(query, hints).OrderByDescending(x => x.Id).ToList();
            //    System.Threading.Thread.Sleep(1000*2);
            //}


            Console.ReadLine();
        }
    }


    [Table(Name = "test0")]
    public class test0
    {
        // 添加PK属性，根据此键修改。 
        [Column(Name = "Id"), PK]
        public int Id { get; set; }


        [Column(Name = "Name")]
        public string Name { get; set; }


        [Column(Name = "Address")]
        public string Address { get; set; }

    }


}
