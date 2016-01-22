namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthorName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "Author_Id" });
            AddColumn("dbo.Comments", "AuthorId", c => c.String());
            AddColumn("dbo.Comments", "AuthorName", c => c.String());
            DropColumn("dbo.Comments", "Author_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "Author_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Comments", "AuthorName");
            DropColumn("dbo.Comments", "AuthorId");
            CreateIndex("dbo.Comments", "Author_Id");
            AddForeignKey("dbo.Comments", "Author_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
