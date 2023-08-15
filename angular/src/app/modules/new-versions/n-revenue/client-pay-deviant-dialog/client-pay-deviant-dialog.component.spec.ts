import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientPayDeviantDialogComponent } from './client-pay-deviant-dialog.component';

describe('ClientPayDeviantDialogComponent', () => {
  let component: ClientPayDeviantDialogComponent;
  let fixture: ComponentFixture<ClientPayDeviantDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClientPayDeviantDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClientPayDeviantDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
