import React, { Component } from 'react';

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = {
            phoneNumber: '',
            password: '',
            status: false,
            message: '',
            loading: false
        };
    }

    handleSubmit = async (e) => {
        e.preventDefault();
        const { phoneNumber, password } = this.state;

        try {
            this.setState({ loading: true });
            const response = await fetch('/api/Authentication/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ phoneNumber, password })
            });

            const data = await response.json();
            this.setState({
                status: data.status,
                message: data.message,
                loading: false
            });
        } catch (error) {
            this.setState({
                status: false,
                message: 'An error occurred while logging in.',
                loading: false
            });
        }
    };

    render() {
        const { phoneNumber, password, loading, status, message } = this.state;

        return (
            <div>
                <h1>Login</h1>
                <form onSubmit={this.handleSubmit}>
                    <div>
                        <label>Phone Number:</label>
                        <input
                            type="text"
                            value={phoneNumber}
                            onChange={(e) => this.setState({ phoneNumber: e.target.value })}
                        />
                    </div>
                    <div>
                        <label>Password:</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => this.setState({ password: e.target.value })}
                        />
                    </div>
                    <button type="submit" disabled={loading}>
                        {loading ? 'Logging in...' : 'Login'}
                    </button>
                </form>
                {status && <p className="success">{message}</p>}
                {!status && <p className="error">{message}</p>}
            </div>
        );
    }
}
