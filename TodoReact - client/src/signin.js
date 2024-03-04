import service from './service.js';
import Form from './form.js';

function Signin() {
    async function verifyUser(user) {
        await service.signin(user);
    }

    function moveToLogin() {
        window.location.href = '/login';
    }

    return (
        <section className="todoapp">
            <header className="header">
                <h1>Sign in</h1>
            </header>
            <Form apiCall={verifyUser} />
            <p className="p">
                enter to an exists account -
                <button className="link" onClick={moveToLogin}>Log In</button>
            </p>
        </section>
    );
}

export default Signin;