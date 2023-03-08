import React from 'react';
import { CaDocGenManual } from '../../components/CaDocGen';

function Home() {
    return (
        <>
            <h1>Conditional Access Documenter</h1>
            <p>Generate a visual PowerPoint presentation of the conditional access policies in your tenant.</p>

            <CaDocGenManual></CaDocGenManual>
        </>
    );
}

export default Home;