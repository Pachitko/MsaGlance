CREATE TABLE users (
    id bigint,
    identity_id bigint,
    chat_id bigint,
    username character varying(256),
    state varchar(256),
    CONSTRAINT "PK_users" PRIMARY KEY (id)
);

CREATE TABLE user_tokens (
    user_id bigint NOT NULL,
    login_provider varchar(256) NOT NULL,
    name varchar(256) NOT NULL,
    value text NULL,
    CONSTRAINT "PK_user_tokens" PRIMARY KEY (user_id, login_provider, name),
    CONSTRAINT "FK_user_tokens_users_UserId" FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);