namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitiativeUsersIDs : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Initiatives", name: "AtlEditor_Id", newName: "AtlEditorID");
            RenameColumn(table: "dbo.Initiatives", name: "AtlExec_Id", newName: "AtlExecID");
            RenameColumn(table: "dbo.Initiatives", name: "NCAEditor_Id", newName: "NCAEditorID");
            RenameColumn(table: "dbo.Initiatives", name: "NCAExec_Id", newName: "NCAExecID");
            RenameColumn(table: "dbo.Initiatives", name: "OntEditor_Id", newName: "OntEditorID");
            RenameColumn(table: "dbo.Initiatives", name: "OntExec_Id", newName: "OntExecID");
            RenameColumn(table: "dbo.Initiatives", name: "PacEditor_Id", newName: "PacEditorID");
            RenameColumn(table: "dbo.Initiatives", name: "PacExec_Id", newName: "PacExecID");
            RenameColumn(table: "dbo.Initiatives", name: "QueEditor_Id", newName: "QueEditorID");
            RenameColumn(table: "dbo.Initiatives", name: "QueExec_Id", newName: "QueExecID");
            RenameColumn(table: "dbo.Initiatives", name: "WstEditor_Id", newName: "wstEditorID");
            RenameColumn(table: "dbo.Initiatives", name: "WstExec_Id", newName: "WstExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_NCAExec_Id", newName: "IX_NCAExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_NCAEditor_Id", newName: "IX_NCAEditorID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_PacExec_Id", newName: "IX_PacExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_PacEditor_Id", newName: "IX_PacEditorID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_WstExec_Id", newName: "IX_WstExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_WstEditor_Id", newName: "IX_wstEditorID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_OntExec_Id", newName: "IX_OntExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_OntEditor_Id", newName: "IX_OntEditorID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_QueExec_Id", newName: "IX_QueExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_QueEditor_Id", newName: "IX_QueEditorID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_AtlExec_Id", newName: "IX_AtlExecID");
            RenameIndex(table: "dbo.Initiatives", name: "IX_AtlEditor_Id", newName: "IX_AtlEditorID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Initiatives", name: "IX_AtlEditorID", newName: "IX_AtlEditor_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_AtlExecID", newName: "IX_AtlExec_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_QueEditorID", newName: "IX_QueEditor_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_QueExecID", newName: "IX_QueExec_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_OntEditorID", newName: "IX_OntEditor_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_OntExecID", newName: "IX_OntExec_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_wstEditorID", newName: "IX_WstEditor_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_WstExecID", newName: "IX_WstExec_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_PacEditorID", newName: "IX_PacEditor_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_PacExecID", newName: "IX_PacExec_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_NCAEditorID", newName: "IX_NCAEditor_Id");
            RenameIndex(table: "dbo.Initiatives", name: "IX_NCAExecID", newName: "IX_NCAExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "WstExecID", newName: "WstExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "wstEditorID", newName: "WstEditor_Id");
            RenameColumn(table: "dbo.Initiatives", name: "QueExecID", newName: "QueExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "QueEditorID", newName: "QueEditor_Id");
            RenameColumn(table: "dbo.Initiatives", name: "PacExecID", newName: "PacExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "PacEditorID", newName: "PacEditor_Id");
            RenameColumn(table: "dbo.Initiatives", name: "OntExecID", newName: "OntExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "OntEditorID", newName: "OntEditor_Id");
            RenameColumn(table: "dbo.Initiatives", name: "NCAExecID", newName: "NCAExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "NCAEditorID", newName: "NCAEditor_Id");
            RenameColumn(table: "dbo.Initiatives", name: "AtlExecID", newName: "AtlExec_Id");
            RenameColumn(table: "dbo.Initiatives", name: "AtlEditorID", newName: "AtlEditor_Id");
        }
    }
}
