﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace SrceApplicaton.Models
{

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class SrceAppDatabase1Entities : DbContext
{
    public SrceAppDatabase1Entities()
        : base("name=SrceAppDatabase1Entities")
    {

    }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }


    public virtual DbSet<Job> Job { get; set; }

    public virtual DbSet<JobTemplates> JobTemplates { get; set; }

    public virtual DbSet<Technician> Technician { get; set; }

    public virtual DbSet<TechnicianStats> TechnicianStats { get; set; }

}

}

