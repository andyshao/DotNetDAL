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

            ////添加
            for (int i = 0; i < 100000; i++)
            {
                dao.Insert<test0>(new test0()
                {
                    Address = "上海" + i,
                    Id = i,
                    Name = "王" + i,
                    CreateDate=DateTime.Now
                });
                Console.WriteLine(i);
                //System.Threading.Thread.Sleep(1000 * 1);
            }

            /////修改
            //dao.Update<test0>(new test0()
            //{
            //    Address = "上海测试",
            //    Id = 1,
            //    Name = "王测试"

            //});


            //分页查询
            //IList<string> shardDb = new List<string> { "0", "1" };
            //IDictionary hints = new Dictionary<string, object>();
            //hints.Add(DALExtStatementConstant.SHARD_IDS, shardDb);
            //var query = dao.GetQuery<test0>().Paging(1, 10, "Id", false);  //.Equal("Name", "王10000");

            //while (true)
            //{
            //    Console.WriteLine("======================================================");
            //    var list_ = dao.SelectList<test0>(query, hints).OrderByDescending(x => x.Id).ToList();
            //    System.Threading.Thread.Sleep(1000 * 3);
            //}

            Console.ReadLine();
        }
    }


    [Table(Name = "test0")]
    public class test0
    {
        [Column(Name = "Id"), PK]
        public int Id { get; set; }


        [Column(Name = "Name")]
        public string Name { get; set; }


        [Column(Name = "Address")]
        public string Address { get; set; }


        [Column(Name = "CreateDate")]
        public DateTime CreateDate { get; set; }

    }


}
