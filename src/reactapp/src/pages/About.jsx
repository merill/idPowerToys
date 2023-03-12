import React from 'react';
import { makeStyles, Title1, Subtitle1 } from '@fluentui/react-components';

const useStyles = makeStyles({
});

function Home() {
    const styles = useStyles();

    return (
        <div className={styles.container}>
            <Title1 as="h1" block>About idPowerToys</Title1>
            <p>idPowerToys is a community project and is not an official Microsoft product or service. We are publishing it under the terms of the <a href="https://github.com/merill/IdPowerToys/blob/main/LICENSE">MIT license.</a></p>

            <Subtitle1 as="h2" block>Feedback and support</Subtitle1>
            <p>Please share feedback on <a href="https://twitter.com/merill">Twitter</a> and report issues on <a href="https://github.com/merill/idPowerToys/issues">GitHub</a>.</p>
            <p>As this is a community project, support if provided on a best efforts basis.</p>

            <Subtitle1 as="h2" block>Credits</Subtitle1>
            <ul>
                <li><a href="https://www.syncfusion.com/">Syncfusion</a> - The Conditional Access documenter will not be possible without the amazing library provided by Syncfusion.</li>
                <li><a href="https://learn.microsoft.com/en-us/graph/toolkit/overview/">Microsoft Graph Toolkit</a> - The components made it super easy to provide a seamless sign in experience for users. </li>
                <li><a href="https://github.com/mattl-msft/Amazing-Icon-Downloader">Amazing Icon Downloader</a> - An amazing tool to find the icons used in the slides.</li>
                <li>Icons8 - <a href="https://icons8.com/icon/1CbCOtKH87xx/we-can-do-it">We Can Do It</a> icon by <a href="https://icons8.com">Icons8</a></li>
            </ul>
        </div>
    );
}

export default Home;