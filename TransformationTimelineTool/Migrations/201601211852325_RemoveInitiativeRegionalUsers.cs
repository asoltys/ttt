namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveInitiativeRegionalUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Initiatives", "AtlEditorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "AtlExecID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "NCAEditorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "NCAExecID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "OntEditorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "OntExecID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "PacEditorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "PacExecID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "QueEditorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "QueExecID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "WstEditorID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "WstExecID", "dbo.AspNetUsers");
            DropIndex("dbo.Initiatives", new[] { "NCAExecID" });
            DropIndex("dbo.Initiatives", new[] { "NCAEditorID" });
            DropIndex("dbo.Initiatives", new[] { "PacExecID" });
            DropIndex("dbo.Initiatives", new[] { "PacEditorID" });
            DropIndex("dbo.Initiatives", new[] { "WstExecID" });
            DropIndex("dbo.Initiatives", new[] { "WstEditorID" });
            DropIndex("dbo.Initiatives", new[] { "OntExecID" });
            DropIndex("dbo.Initiatives", new[] { "OntEditorID" });
            DropIndex("dbo.Initiatives", new[] { "QueExecID" });
            DropIndex("dbo.Initiatives", new[] { "QueEditorID" });
            DropIndex("dbo.Initiatives", new[] { "AtlExecID" });
            DropIndex("dbo.Initiatives", new[] { "AtlEditorID" });
            DropColumn("dbo.Initiatives", "NCAExecID");
            DropColumn("dbo.Initiatives", "NCAEditorID");
            DropColumn("dbo.Initiatives", "PacExecID");
            DropColumn("dbo.Initiatives", "PacEditorID");
            DropColumn("dbo.Initiatives", "WstExecID");
            DropColumn("dbo.Initiatives", "WstEditorID");
            DropColumn("dbo.Initiatives", "OntExecID");
            DropColumn("dbo.Initiatives", "OntEditorID");
            DropColumn("dbo.Initiatives", "QueExecID");
            DropColumn("dbo.Initiatives", "QueEditorID");
            DropColumn("dbo.Initiatives", "AtlExecID");
            DropColumn("dbo.Initiatives", "AtlEditorID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Initiatives", "AtlEditorID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "AtlExecID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "QueEditorID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "QueExecID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "OntEditorID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "OntExecID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "WstEditorID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "WstExecID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "PacEditorID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "PacExecID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "NCAEditorID", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "NCAExecID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Initiatives", "AtlEditorID");
            CreateIndex("dbo.Initiatives", "AtlExecID");
            CreateIndex("dbo.Initiatives", "QueEditorID");
            CreateIndex("dbo.Initiatives", "QueExecID");
            CreateIndex("dbo.Initiatives", "OntEditorID");
            CreateIndex("dbo.Initiatives", "OntExecID");
            CreateIndex("dbo.Initiatives", "WstEditorID");
            CreateIndex("dbo.Initiatives", "WstExecID");
            CreateIndex("dbo.Initiatives", "PacEditorID");
            CreateIndex("dbo.Initiatives", "PacExecID");
            CreateIndex("dbo.Initiatives", "NCAEditorID");
            CreateIndex("dbo.Initiatives", "NCAExecID");
            AddForeignKey("dbo.Initiatives", "WstExecID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "WstEditorID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "QueExecID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "QueEditorID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "PacExecID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "PacEditorID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "OntExecID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "OntEditorID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "NCAExecID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "NCAEditorID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "AtlExecID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "AtlEditorID", "dbo.AspNetUsers", "Id");
        }
    }
}
