namespace JsonInterfaceSerialize.Utilities.Constants
{
    public class Gen
    {
        public const int DB_CMD_TIMEOUT_DEFAULT = 300;
        public const int ROW_COUNT_ERROR = -10;

        public const bool SKIP_SSL_CERTIFICATION = true;

        public const string DB_CON_STR_KEY = "db_constr_template";
        public const string DB_CRAQEN_CON_STR_KEY = "db_constr_craqen";
        public const string DB_CON_TIMEOUT_KEY = "db_timeout_con_secs";
        public const string DB_CMD_TIMEOUT_KEY = "db_timeout_cmd_secs";


        public const string ES_BASEURL_KEY = "es_base_url";
        public const string ES_DIDURL_KEY = "es_didurl_key";
        public const string ES_API_KEY = "es_api_key";
        public const string ES_SUBSTANCE_SEARCH_DID = "es_substance_search_did";
        public const string ES_SUBSTANCE_SEARCH_ID = "es_substance_search_id";
        public const string ES_REGULATION_SEARCH_ID = "es_regulation_search_id";
        public const string ES_TEXT_SEARCH_ID = "es_text_search_id";


        public const string ES_USER = "es_user";
        public const string ES_PASSWORD = "es_password";
        public const string ES_USE_SSL = "es_use_ssl";

        public const string ES_DEFAULT_PAGE_NUM = "es_default_page_num";
        public const string ES_DEFAULT_PAGE_SIZE = "es_default_page_size";

        public const string SSO_PRODUCT_GCOMPLY = "sso_product_gcomply";
        public const string SSO_BASE_URI = "sso_base_uri";
        public const string SSO_PRODUCT_LIST_URI = "sso_product_list_uri";
        public const string BEARERTOKEN = "bearertoken";
    }
}
