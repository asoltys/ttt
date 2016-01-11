namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitiativeUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Initiatives", "AtlEditor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "AtlExec_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "NCAEditor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "NCAExec_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "OntEditor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "OntExec_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "PacEditor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "PacExec_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "QueEditor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "QueExec_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "WstEditor_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Initiatives", "WstExec_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Initiatives", "AtlEditor_Id");
            CreateIndex("dbo.Initiatives", "AtlExec_Id");
            CreateIndex("dbo.Initiatives", "NCAEditor_Id");
            CreateIndex("dbo.Initiatives", "NCAExec_Id");
            CreateIndex("dbo.Initiatives", "OntEditor_Id");
            CreateIndex("dbo.Initiatives", "OntExec_Id");
            CreateIndex("dbo.Initiatives", "PacEditor_Id");
            CreateIndex("dbo.Initiatives", "PacExec_Id");
            CreateIndex("dbo.Initiatives", "QueEditor_Id");
            CreateIndex("dbo.Initiatives", "QueExec_Id");
            CreateIndex("dbo.Initiatives", "WstEditor_Id");
            CreateIndex("dbo.Initiatives", "WstExec_Id");
            AddForeignKey("dbo.Initiatives", "AtlEditor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "AtlExec_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "NCAEditor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "NCAExec_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "OntEditor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "OntExec_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "PacEditor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "PacExec_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "QueEditor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "QueExec_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "WstEditor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Initiatives", "WstExec_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Initiatives", "WstExec_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "WstEditor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "QueExec_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "QueEditor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "PacExec_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "PacEditor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "OntExec_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "OntEditor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "NCAExec_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "NCAEditor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "AtlExec_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Initiatives", "AtlEditor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Initiatives", new[] { "WstExec_Id" });
            DropIndex("dbo.Initiatives", new[] { "WstEditor_Id" });
            DropIndex("dbo.Initiatives", new[] { "QueExec_Id" });
            DropIndex("dbo.Initiatives", new[] { "QueEditor_Id" });
            DropIndex("dbo.Initiatives", new[] { "PacExec_Id" });
            DropIndex("dbo.Initiatives", new[] { "PacEditor_Id" });
            DropIndex("dbo.Initiatives", new[] { "OntExec_Id" });
            DropIndex("dbo.Initiatives", new[] { "OntEditor_Id" });
            DropIndex("dbo.Initiatives", new[] { "NCAExec_Id" });
            DropIndex("dbo.Initiatives", new[] { "NCAEditor_Id" });
            DropIndex("dbo.Initiatives", new[] { "AtlExec_Id" });
            DropIndex("dbo.Initiatives", new[] { "AtlEditor_Id" });
            DropColumn("dbo.Initiatives", "WstExec_Id");
            DropColumn("dbo.Initiatives", "WstEditor_Id");
            DropColumn("dbo.Initiatives", "QueExec_Id");
            DropColumn("dbo.Initiatives", "QueEditor_Id");
            DropColumn("dbo.Initiatives", "PacExec_Id");
            DropColumn("dbo.Initiatives", "PacEditor_Id");
            DropColumn("dbo.Initiatives", "OntExec_Id");
            DropColumn("dbo.Initiatives", "OntEditor_Id");
            DropColumn("dbo.Initiatives", "NCAExec_Id");
            DropColumn("dbo.Initiatives", "NCAEditor_Id");
            DropColumn("dbo.Initiatives", "AtlExec_Id");
            DropColumn("dbo.Initiatives", "AtlEditor_Id");
        }
    }
}
