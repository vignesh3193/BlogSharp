namespace BlogSharp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        creation = c.DateTime(nullable: false),
                        birthday = c.DateTime(nullable: false),
                        blogName = c.String(nullable: false),
                        location = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        isPrivate = c.Boolean(nullable: false),
                        bio = c.String(nullable: false),
                        BlogPost_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPost_Id)
                .Index(t => t.BlogPost_Id);
            
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(nullable: false),
                        dateCreated = c.DateTime(nullable: false),
                        title = c.String(),
                        content = c.String(),
                        Person_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .Index(t => t.PersonId)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlogPostId = c.Int(nullable: false),
                        Author = c.String(nullable: false),
                        contents = c.String(nullable: false),
                        dateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPostId, cascadeDelete: true)
                .Index(t => t.BlogPostId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlogPostId = c.Int(nullable: false),
                        username = c.String(nullable: false),
                        ratingNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPostId, cascadeDelete: true)
                .Index(t => t.BlogPostId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        tagName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PersonPersons",
                c => new
                    {
                        Person_Id = c.Int(nullable: false),
                        Person_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Person_Id1 })
                .ForeignKey("dbo.People", t => t.Person_Id)
                .ForeignKey("dbo.People", t => t.Person_Id1)
                .Index(t => t.Person_Id)
                .Index(t => t.Person_Id1);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogPosts", "Person_Id", "dbo.People");
            DropForeignKey("dbo.People", "BlogPost_Id", "dbo.BlogPosts");
            DropForeignKey("dbo.TagBlogPosts", "BlogPost_Id", "dbo.BlogPosts");
            DropForeignKey("dbo.TagBlogPosts", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Ratings", "BlogPostId", "dbo.BlogPosts");
            DropForeignKey("dbo.BlogPosts", "PersonId", "dbo.People");
            DropForeignKey("dbo.Comments", "BlogPostId", "dbo.BlogPosts");
            DropForeignKey("dbo.PersonPersons", "Person_Id1", "dbo.People");
            DropForeignKey("dbo.PersonPersons", "Person_Id", "dbo.People");
            DropIndex("dbo.TagBlogPosts", new[] { "BlogPost_Id" });
            DropIndex("dbo.TagBlogPosts", new[] { "Tag_Id" });
            DropIndex("dbo.PersonPersons", new[] { "Person_Id1" });
            DropIndex("dbo.PersonPersons", new[] { "Person_Id" });
            DropIndex("dbo.Ratings", new[] { "BlogPostId" });
            DropIndex("dbo.Comments", new[] { "BlogPostId" });
            DropIndex("dbo.BlogPosts", new[] { "Person_Id" });
            DropIndex("dbo.BlogPosts", new[] { "PersonId" });
            DropIndex("dbo.People", new[] { "BlogPost_Id" });
            DropTable("dbo.TagBlogPosts");
            DropTable("dbo.PersonPersons");
            DropTable("dbo.Tags");
            DropTable("dbo.Ratings");
            DropTable("dbo.Comments");
            DropTable("dbo.BlogPosts");
            DropTable("dbo.People");
        }
    }
}
