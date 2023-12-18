using System;
using System.Linq;
using System.Reflection;

namespace lab3_task2
{
    public enum execution_status : int
    {
        status_success = 0,
        status_argument_incorrect = 1,
        status_internal_error = 2,
    }

    class user_interface
    {
        private string _help_link;
        private bool _debug_mode;

        public execution_status setup_error_handler(string help_link, bool debug_mode = false)
        {
            if (string.IsNullOrEmpty(help_link))
                return execution_status.status_argument_incorrect;

            this._help_link = help_link;
            this._debug_mode = debug_mode;

            return execution_status.status_success;
        }

        private Exception _build_exception(string exception_title, string source)
        {
            Exception _ex = new Exception(exception_title);

            _ex.HelpLink = this._help_link;
            _ex.Source = _debug_mode ? source : "";

            return _ex;
        }

        private execution_status _error_handler(MethodInfo method, Type type, object obj, object[] parameters)
        {
            if (method == null)
                return execution_status.status_argument_incorrect;

            try
            {
                method = method.MakeGenericMethod(type);

                if (!(Boolean)method.Invoke(obj, parameters))
                    return execution_status.status_internal_error;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.InnerException.Source} ( {ex.InnerException.Message} )\nHelp link: {ex.InnerException.HelpLink}");
                Console.ReadKey();
                return execution_status.status_internal_error;
            }

            return execution_status.status_success;
        }

        private MethodInfo get_function_by_name(Type _type, string _function_name)
        {
            if (string.IsNullOrEmpty(_function_name))
                return null;

            return (from _method in _type.GetMethods() where _method.Name == _function_name select _method).First();
        }

        private bool _get_user_input<T>(string _head, out T value)
        {
            Type _type_info = typeof(T);
            MethodInfo _TryParse = get_function_by_name(_type_info, "TryParse");

            if (_TryParse == null)
                throw _build_exception("Unsupported data type.", "_get_user_input throw error at line 82");

            Console.Write(_head);
            string _user_input = Console.ReadLine();

            if (string.IsNullOrEmpty(_user_input))
                throw _build_exception("Input is null or empty.", "_get_user_input throw error at line 88");

            object[] _args = { _user_input, null };

            if (!(Boolean)_TryParse.Invoke(null, _args))
                throw _build_exception("Input incorrect.", "_get_user_input throw error at line 93");

            value = (T)_args[1];

            return true;
        }
        public execution_status get_user_input<T>(string title, out T value)
        {
            object[] _args = { title, null };
            value = default;

            execution_status status = _error_handler(this.GetType().GetMethod("_get_user_input", BindingFlags.Instance | BindingFlags.NonPublic), typeof(T), this, _args);

            if (status == execution_status.status_success)
                value = (T)_args[1];

            return status;
        }
    }
}
