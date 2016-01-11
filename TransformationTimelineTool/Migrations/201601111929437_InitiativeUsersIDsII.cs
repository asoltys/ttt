namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitiativeUsersIDsII : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Initiatives", new[] { "wstEditorID" });
            CreateIndex("dbo.Initiatives", "WstEditorID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Initiatives", new[] { "WstEditorID" });
            CreateIndex("dbo.Initiatives", "wstEditorID");
        }
    }
}
