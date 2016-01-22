namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropAuthorID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Comments", "AuthorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "AuthorId", c => c.String());
        }
    }
}
