import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';


import './custom.css'
import HeySearch from './components/HeySearch';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' render={(props) => <HeySearch {...props} url={'/api'} />} />
        <Route path='/heysearch' render={(props) => <HeySearch {...props} url={'/api'} />} />
      </Layout>
    );
  }
}
