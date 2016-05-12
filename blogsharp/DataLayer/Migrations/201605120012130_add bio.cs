namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbio : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "bio", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "bio");
        }
    }
}
