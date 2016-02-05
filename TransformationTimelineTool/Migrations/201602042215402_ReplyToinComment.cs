namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplyToinComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "ReplyTo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "ReplyTo");
        }
    }
}
