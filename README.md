example 

execution_status status;

user_interface _user_interface = new user_interface();
_user_interface.setup_error_handler("your help link", true);

int _year = default;

status = _user_interface.get_user_input<int>("enter year:", out _year);
if (status != execution_status.status_success) return;
