CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE roles (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" character varying(256) NULL,
    "NormalizedName" character varying(256) NULL,
    "ConcurrencyStamp" text NULL,
    CONSTRAINT "PK_roles" PRIMARY KEY ("Id")
);

CREATE TABLE users (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "UserName" character varying(256) NULL,
    "NormalizedUserName" character varying(256) NULL,
    "Email" character varying(256) NULL,
    "NormalizedEmail" character varying(256) NULL,
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text NULL,
    "SecurityStamp" text NULL,
    "ConcurrencyStamp" text NULL,
    "PhoneNumber" text NULL,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone NULL,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    CONSTRAINT "PK_users" PRIMARY KEY ("Id")
);

CREATE TABLE role_claims (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "RoleId" bigint NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_role_claims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_role_claims_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles ("Id") ON DELETE CASCADE
);

CREATE TABLE user_claims (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId" bigint NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_user_claims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_user_claims_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
);

CREATE TABLE user_logins (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text NULL,
    "UserId" bigint NOT NULL,
    CONSTRAINT "PK_user_logins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_user_logins_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
);

CREATE TABLE user_roles (
    "UserId" bigint NOT NULL,
    "RoleId" bigint NOT NULL,
    CONSTRAINT "PK_user_roles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_user_roles_roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES roles ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_user_roles_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
);

CREATE TABLE user_tokens (
    "UserId" bigint NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text NULL,
    CONSTRAINT "PK_user_tokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_user_tokens_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_role_claims_RoleId" ON role_claims ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON roles ("NormalizedName");

CREATE INDEX "IX_user_claims_UserId" ON user_claims ("UserId");

CREATE INDEX "IX_user_logins_UserId" ON user_logins ("UserId");

CREATE INDEX "IX_user_roles_RoleId" ON user_roles ("RoleId");

CREATE INDEX "EmailIndex" ON users ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON users ("NormalizedUserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220610160234_Init AuthDbContext', '6.0.5');

COMMIT;


