CREATE TABLE "Users" (
    "Id" int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "Login" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
	"Email" TEXT,
	"PhoneNumber" TEXT,
    "RoleId" INTEGER NOT NULL,
    "IsBlocked" BOOLEAN NOT NULL,
    "RegistrationDate" TIMESTAMP NOT NULL
);

CREATE UNIQUE INDEX "Unique_Users_Login" ON "Users" ("Login");

CREATE TABLE "Tokens" (
    "Id" int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "AccessToken" TEXT NOT NULL,
    "RefreshToken" TEXT NOT NULL,
    "UserId" int NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "AccessTokenExpiresAt" TIMESTAMP NOT NULL,
    "RefreshTokenExpiresAt" TIMESTAMP NOT NULL,
    "IpAddress" TEXT,
    "UserAgent" TEXT,
    CONSTRAINT fk_user FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

