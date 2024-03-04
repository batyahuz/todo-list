import React, { useState } from 'react';

function Form({ apiCall = async () => { } }) {
    const [name, setName] = useState("");
    const [password, setPassword] = useState("");

    async function verifyUser(e) {
        e.preventDefault();
        await apiCall({ name, password });
        setName("");
        setPassword("");
        window.location.href = '/';
    }

    return (
        <form onSubmit={verifyUser}>
            <input className="new-todo" placeholder="user name" value={name} onChange={(e) => setName(e.target.value)} />
            <input className="new-todo" placeholder="user password" value={password} onChange={(e) => setPassword(e.target.value)} />
            <input type="submit" className="new-todo submit" />
        </form>
    );
}

export default Form;