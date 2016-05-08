namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagBlogchangedtoManytoMany : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tags", "BlogPost_Id", "dbo.BlogPosts");
            DropIndex("dbo.Tags", new[] { "BlogPost_Id" });
            CreateTable(
                "dbo.TagBlogPosts",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        BlogPost_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.BlogPost_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPost_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.BlogPost_Id);
            
            DropColumn("dbo.Tags", "BlogPost_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tags", "BlogPost_Id", c => c.Int());
            DropForeignKey("dbo.TagBlogPosts", "BlogPost_Id", "dbo.BlogPosts");
            DropForeignKey("dbo.TagBlogPosts", "Tag_Id", "dbo.Tags");
            DropIndex("dbo.TagBlogPosts", new[] { "BlogPost_Id" });
            DropIndex("dbo.TagBlogPosts", new[] { "Tag_Id" });
            DropTable("dbo.TagBlogPosts");
            CreateIndex("dbo.Tags", "BlogPost_Id");
            AddForeignKey("dbo.Tags", "BlogPost_Id", "dbo.BlogPosts", "Id");
        }
    }
}
