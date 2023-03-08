import React from "react";
import { NavBar } from "./NavBar"
import { Login } from '@microsoft/mgt-react';
import { Link, Outlet } from "react-router-dom";

export const PageLayout = (props) => {
    return (
        <>
            <div id="container">
                <header>

                    <div className="logo" role="presentation" aria-hidden="true" itemProp="logo" itemScope="itemScope">
                        <img src="/idPowerToysLogo.png" height="46"></img>
                    </div>
                    <div className="colorgroup"></div>
                    <div className="profile">
                        {/* {isAuthenticated ? <SignOutButton /> : <SignInButton />} */}
                        <Login />
                    </div>
                </header>

                <main>
                    <nav>
                        <NavBar />
                    </nav>
                    <div className="content">
                        <article id="article">
                            <Outlet />
                            {/* {props.children} */}
                        </article>
                        <aside>
                        </aside>
                    </div>
                </main>
                <footer>
                    <div className="switches">
                        <small>
                            Brought to you by &nbsp;<a target="_blank" href="https://merill.net/">Merill</a>
                        </small>
                    </div>
                    <div className="version">
                    </div>
                    <div className="copy">
                        <small>
                            <Link to="/about">About</Link>
                            &nbsp;|&nbsp;<a target="_blank" href="https://merill.net/privacy">Privacy Policy</a>
                        </small>
                    </div>
                </footer>
            </div>
        </>
    );
};