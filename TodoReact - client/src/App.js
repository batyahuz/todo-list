import { BrowserRouter, Route, Routes } from "react-router-dom";
import Signin from "./signin";
import Todos from "./todos";
import Login from './login';
import service from './service';
import { useEffect, useState } from "react";

function App() {
    const [connected, setConnected] = useState(service.connected());

    function SignOut() {
        service.signout()
        setConnected("")
        window.location.href = '/login';
    }

    useEffect(() => {
        setConnected(service.connected())
    }, [service.connected()]);

    return (
        <>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Todos />} />
                    <Route path="/signin" element={<Signin />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/*" element={<Todos />} />
                </Routes>
            </BrowserRouter>
            <footer>
                {connected ? <button className="link" onClick={SignOut}>Sign Out</button> : ""}
            </footer>
        </>
    );
}

export default App;
