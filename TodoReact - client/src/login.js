import service from './service.js';
import Form from './form.js';

function Login() {
    async function verifyUser(user) {
        await service.login(user);
    }

    function moveToSignin() {
        window.location.href = '/signin';
    }

    return (
        <section className="todoapp">
            <header className="header">
                <h1>Login</h1>
            </header>
            <Form apiCall={verifyUser} />
            <p className="p">
                to open a new account -
                <button className="link" onClick={moveToSignin}>Sign In</button>
            </p>
        </section>
    );
}

export default Login;