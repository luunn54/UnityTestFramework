coroutine.yield(WaitUILoad("LoginScreen"));

coroutine.yield(DelaySecond(2));

coroutine.yield(InputTextField("ipf_user", "luu"));

coroutine.yield(InputTextField("ipf_pass", "12345"));

MockupApi("http://127.0.0.1:8080/login", '{"status":200,"data":{"user":{"name":"Nguyen Van XXX","pw":"1234","diamond":10}}}');

coroutine.yield(ClickObject("btn_login"));

coroutine.yield(WaitUILoad("HomeScreen"));

coroutine.yield(DelaySecond(2));

Assert(GetText("txt_diamond_number") == "10", "txt_diamond_number is not '10'");

Assert(GetText("txt_name") == "Nguyen Van XXX", "txt_name is not 'Nguyen Van XXX'");

Assert(CanClick("room_5"), "can select room_5");

Assert(CanClick("room_10"), "can select room_10");

Assert(not CanClick("btn_save"), "can not select btn_save");

