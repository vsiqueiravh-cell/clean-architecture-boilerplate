CREATE TABLE IF NOT EXISTS projects (
    "Id" uuid PRIMARY KEY,
    "Key" varchar(12) NOT NULL UNIQUE,
    "Name" varchar(160) NOT NULL,
    "Description" varchar(800) NULL,
    "IsArchived" boolean NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "CreatedBy" varchar(160) NOT NULL,
    "LastModifiedAt" timestamptz NULL,
    "LastModifiedBy" varchar(160) NULL
);

CREATE TABLE IF NOT EXISTS project_tasks (
    "Id" uuid PRIMARY KEY,
    "ProjectId" uuid NOT NULL REFERENCES projects ("Id") ON DELETE CASCADE,
    "Title" varchar(180) NOT NULL,
    "Description" varchar(800) NULL,
    "DueDate" date NULL,
    "Priority" varchar(24) NOT NULL,
    "Status" varchar(24) NOT NULL,
    "CompletedAt" timestamptz NULL,
    "CreatedAt" timestamptz NOT NULL,
    "CreatedBy" varchar(160) NOT NULL,
    "LastModifiedAt" timestamptz NULL,
    "LastModifiedBy" varchar(160) NULL
);

CREATE INDEX IF NOT EXISTS ix_project_tasks_project_id_status
    ON project_tasks ("ProjectId", "Status");
