CREATE TABLE users (
    id bigint,
    identity_id bigint,
    chat_id bigint,
    username character varying(256),
    state character varying(256),
    CONSTRAINT "PK_users" PRIMARY KEY (id)
);